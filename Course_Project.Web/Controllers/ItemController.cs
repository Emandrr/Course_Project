using Azure;
using Course_Project.Application.Interfaces;
using Course_Project.Application.Services;
using Course_Project.Application.Utils;
using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Course_Project.Web.Controllers
{
    public class ItemController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IItemService _itemService;
        private readonly ICloudService _cloudService;
        public ItemController(IInventoryService inventoryService, IItemService itemService, ICloudService cloudService)
        {
            _inventoryService = inventoryService;
            _itemService = itemService;
            _cloudService = cloudService;
        }

        [HttpGet]
        public async Task<IActionResult> Add(Guid InventoryId)
        {
            var response = await _inventoryService.GetInventoryAsNoTrackingAsync(InventoryId.ToString());
            if (response != null && (await Check(response) || await _inventoryService.CanEdit(response, User.Identity.Name)))
                return View(new ItemAddViewModel()
                {
                    Inventory = response,
                    CreatorName = User.Identity.Name,
                    Ids = Generator.GenerateExample(response.CustomSetOfIds),
                    ReturnUrl = Request.Headers["Referer"].ToString()
                });
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ItemEditViewModel model, int Version)
        {
            var response = await _inventoryService.GetInventoryAsNoTrackingAsync(model.InvId);
            model.Inventory = response;
            model.Item = await _itemService.GetItemAsync(model.ItmId);
            if(model.Item.Version!=Version) return RedirectToAction("Information", "Inventory", new { id = response.PublicId.ToString() });
            if (!Validator.Validate(model.CustomIdString, response.CustomSetOfIds, response.Items.Count()))
            {
                ModelState.AddModelError(string.Empty, "Invalid id");
                return View(model);
            }
            List<CustomField> lst = Parser.ParseFields(model.CustomFields, response.Id);
            if (ModelState.IsValid && (await Check(response) || await _inventoryService.CanEdit(response, User.Identity.Name))
                && await _itemService.CheckOnCustomId(model.CustomIdString, model.ItmId, model.InvId))
            {
                await _itemService.EditItemAsync(model.Name, lst, model.CustomIdString, model.ImageFile, model.ItmId);
                return RedirectToAction("Information", "Inventory", new { id = response.PublicId.ToString() });
            }
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id, bool IsEdit)
        {
            bool value = false;
            if (User.Identity.IsAuthenticated) value = true;
            var response = await _itemService.GetItemAsNoTrackingAsync(id);
            if (response != null) return View(new ItemEditViewModel() { Item = response, HasChance = value, 
                IsSet = await _itemService.HasLikeAsync(User.Identity.Name, 
                response.PublicId), Photo = await _cloudService.GetPhotoAsync(response.PhotoLink), 
                Inventory = await _inventoryService.GetInventoryAsNoTrackingAsync(response.Inventory.PublicId.ToString()),
                ReturnUrl = Request.Headers["Referer"].ToString()
            });
            else return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Add(ItemAddViewModel model)
        {
            var response = await _inventoryService.GetInventoryAsNoTrackingAsync(model.InvId);
            model.Inventory = response;
            if (!Validator.Validate(model.CustomIdString, response.CustomSetOfIds, response.Items.Count()))
            {
                ModelState.AddModelError(string.Empty, "Invalid id");
                return View(model);
            }
            List<CustomField> lst = Parser.ParseFields(model.CustomFields, response.Id);
            if (ModelState.IsValid && (await Check(response) || await _inventoryService.CanEdit(response, User.Identity.Name))
                && await _itemService.CheckOnCustomId(model.CustomIdString, null, model.InvId.ToString()))
            {
                await _itemService.CreateAsync(model.Inventory.Id, model.Name, model.CreatorName, lst, model.CustomIdString, model.ImageFile);
                return RedirectToAction("Information", "Inventory", new { id = response.PublicId.ToString() });
            }
            return View(model);
        }
        public async Task<IActionResult> Information(string id)
        {
            bool value = false;
            if (User.Identity.IsAuthenticated) value = true;
            var response = await _itemService.GetItemAsNoTrackingAsync(id);
            var inv = await _inventoryService.GetInventoryAsNoTrackingAsync(response.Inventory.PublicId.ToString());
            if (response != null) return View(new ItemViewModel()
            {
                Item = response,
                HasChance = value,
                IsSet = await _itemService.HasLikeAsync(User.Identity.Name, response.PublicId),
                Photo = await _cloudService.GetPhotoAsync(response.PhotoLink),
                IsEdit = (await Check(inv) ||
                await _inventoryService.CanEdit(inv, User.Identity.Name) || response.CreatorName == User.Identity.Name),
                Inventory = inv,
                ReturnUrl = Request.Headers["Referer"].ToString()
            });
            else return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(string CreatorName, Guid ItmId, bool IsSet)
        {
            if (await _itemService.HasLikeAsync(CreatorName, ItmId)) await _itemService.RemoveLike(CreatorName, ItmId);
            else await _itemService.SetLike(CreatorName, ItmId);
            return RedirectToAction("Information", new { id = ItmId });
        }
        private async Task<bool> Check(Inventory inventory)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated) return false;
            if (User.Identity.Name == inventory.User.UserName ||
                User.IsInRole("Admin")) return true;
            return false;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(Guid[] PublicId, Guid InvId)
        {
            await _itemService.DeleteSelectedAsync(PublicId);
            return RedirectToAction("Details", "Inventory", new { id = InvId.ToString() });
        }
    }
}

