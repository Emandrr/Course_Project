using Course_Project.Application.Contracts.DTO;
using Course_Project.Application.Interfaces;
using Course_Project.Application.Services;
using Course_Project.Application.Utils;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Course_Project.Web.Controllers
{

        public class InventoryController : Controller
        {
            private readonly IInventoryService _inventoryService;
            private readonly ICategoryService _categoryService;
            private readonly UserManager<User> _userManager;
            private readonly IUserService _userService;
            private readonly ICloudService _cloudService;
            private readonly ITagService _tagService;
            public InventoryController(IInventoryService inventoryService, UserManager<User> userManager, IUserService userService, ICloudService cloudService, ICategoryService categoryService, ITagService tagService)
            {
                _inventoryService = inventoryService;
                _userManager = userManager;
                _userService = userService;
                _cloudService = cloudService;
                _categoryService = categoryService;
                _tagService = tagService;
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> Create(InventoryCreateViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    model.Categories = await _categoryService.GetCategoriesAsync();
                    return View(model);
                }
                Guid res = await _inventoryService.CreateAsync(model.Title, model.Description,
                    model.IsPublic, new List<CustomIdRule>() { new CustomIdRule() { Rule = "zeros", IdType = IdType.Rand20Bit } },
                    new List<Item>(), model.CustomElems, model.ImageFile, model.UserId, model.SelectedCategoryId);
                if (model.Tags != null) await _tagService.TryAddTagsToInventory(res.ToString(), System.Text.Json.JsonSerializer.Deserialize<List<string>>(model.Tags) ?? new List<string>());
                return RedirectToAction("Details", "Inventory", new { id = res.ToString() });
            }
            [Authorize]
            [HttpGet]
            public async Task<IActionResult> Create(string id)
            {
                return View(new InventoryCreateViewModel() { UserId = id, Categories = await _categoryService.GetCategoriesAsync(), SelectedCategoryId = 1, AllTags = await _tagService.GetAllTagsNamesAcync() });
            }

            public async Task<IActionResult> All()
            {
                return View(await _inventoryService.GetAllAsync());
            }
            [Authorize]
            public async Task<IActionResult> Details(string id)
            {
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(id);
                if (response != null && (await Check(response) || await _inventoryService.CanEdit(response, User.Identity.Name)))
                    return View(new InventoryEditViewModel()
                    {
                        inventory = response,
                        Photo = await _cloudService.GetPhotoAsync(response.PhotoLink),
                        Ids = Generator.GenerateExample(response.CustomSetOfIds),
                        Users = await _userService.GetAllUsersAsync(),
                        UserAccess = await _inventoryService.GetAccess(response.PublicId),
                        ReturnUrl = Request.Headers["Referer"].ToString(),
                        SelectedCategoryId = response.Category.Id,
                        Categories = await _categoryService.GetCategoriesAsync(),
                        AllTags = await _tagService.GetAllTagsNamesAcync(),
                        InvTags = await _tagService.GetAllTagsNamesForInventoryAsync(response.PublicId.ToString())
                    });
                else return RedirectToAction("Index", "Home");
            }
            public async Task<IActionResult> Information(string id)
            {
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(id);
                if (response != null) return View(new InventoryInfoViewModel()
                {
                    inventory = response,
                    Photo = await _cloudService.GetPhotoAsync(response.PhotoLink),
                    ReturnUrl = Request.Headers["Referer"].ToString(),
                    UserAccess = await _inventoryService.GetAccess(response.PublicId),
                    user = User.Identity.Name == null ? null : await _userManager.FindByNameAsync(User.Identity.Name)
                });
                else return RedirectToAction("Index", "Home");
            }
            [Authorize]
            [HttpPost]
            public async Task<JsonResult> UpdateElems(List<CustomElem> customElems, Guid inventoryId,int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(inventoryId.ToString());
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(inventoryId.ToString());
                if (inventory == null || !(await Check(inventory)) || Version!=response.Version) return Json(new { error = "Forbidden" });
                if (ModelState.IsValid)
                {
                    await _inventoryService.UpdateCustomElemsAsync(customElems, inventory);
                    return Json(new { success = true, newVersion = inventory.Version });
                }
                Response.StatusCode = 403;
                return Json(new { error = "Forbidden" });
            }
            [Authorize]
            [HttpPost]
            public async Task<JsonResult> UpdateCustomId(List<CustomSetOfId> customSetOfIds, List<string> ids, Guid inventoryId,int Version)
            {
            Inventory inventory = await _inventoryService.GetInventoryAsync(inventoryId.ToString());
            var response = await _inventoryService.GetInventoryAsNoTrackingAsync(inventoryId.ToString());
            if (inventory == null || !(await Check(inventory)) || Version != response.Version) return Json(new { error = "Forbidden" });
                await _inventoryService.UpdateCustomSetOfIdsAsync(customSetOfIds, inventory);
                return Json(new { success = true, newVersion = inventory.Version });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> MakePublic(Guid id,int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(id.ToString()); 
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(id.ToString());
                if (inventory == null || !(await Check(inventory)) || Version != response.Version) return RedirectToAction("Index", "Home");
                inventory.IsPublic = true;
                await _inventoryService.UpdateAsync(inventory);
                return RedirectToAction("Details", new { id = id.ToString() });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> MakePrivate(Guid id,int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(id.ToString());
            var response = await _inventoryService.GetInventoryAsNoTrackingAsync(id.ToString());
            if (inventory == null || !(await Check(inventory)) || Version != response.Version) return RedirectToAction("Index", "Home");
                inventory.IsPublic = false;
                await _inventoryService.UpdateAsync(inventory);
                return RedirectToAction("Details", new { id = id.ToString() });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> GiveAccess(string[] UserId, Guid id,int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(id.ToString());
                if (inventory == null || !(await Check(inventory)) || Version != inventory.Version) return RedirectToAction("Index", "Home");
                await _inventoryService.GiveAccessSelectedAsync(UserId, id);
                return RedirectToAction("Details", new { id = id.ToString() });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> TakeAccess(string[] UserId, Guid id, int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(id.ToString());
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(id.ToString());
                if (inventory == null || !(await Check(inventory)) || Version != response.Version)return  RedirectToAction("Index", "Home");
                await _inventoryService.TakeAccessSelectedAsync(UserId, id);
                return RedirectToAction("Details", new { id = id.ToString() });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> Save(SaveInventory saveInventory, int Version)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(saveInventory.Id.ToString());
                var response = await _inventoryService.GetInventoryAsNoTrackingAsync(saveInventory.Id.ToString());
                if (inventory == null || !(await Check(inventory)) || Version != response.Version) return RedirectToAction("Index", "Home");
                await _inventoryService.UpdateMainFieldsAsync(saveInventory.Title, saveInventory.Description, saveInventory.ImageFile, saveInventory.SelectedCategoryId, inventory);
                await _tagService.UpdateTagsForInventoryAsync(inventory.PublicId.ToString(), System.Text.Json.JsonSerializer.Deserialize<List<string>>(saveInventory.InvTags) ?? new List<string>());
                return RedirectToAction("Details", new { id = saveInventory.Id.ToString() });
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> Delete(Guid id)
            {
                Inventory inventory = await _inventoryService.GetInventoryAsync(id.ToString());
                if (inventory == null || !(await Check(inventory))) RedirectToAction("Index", "Home");
                await _inventoryService.DeleteAsync(inventory);
                return RedirectToAction("Index", "Home");
            }
            private async Task<bool> Check(Inventory inventory)
            {
                if (User.Identity == null || !User.Identity.IsAuthenticated) return false;
                if (User.Identity.Name == inventory.User.UserName ||
                    User.IsInRole("Admin")) return true;
                return false;
            }
            [HttpPost]
            public async Task<IActionResult> Search(string query, string returnUrl, string queryStr)
            {
                var rsp = await _inventoryService.TrySearchAsync(query);
                if (rsp.Count != 0) return View(rsp);
                else return LocalRedirect(returnUrl + queryStr);
            }


            [Authorize, HttpPost]
            public async Task<IActionResult> DeleteSelected(Guid[] InventoryId, string UserName)
            {
                await _inventoryService.DeleteSelectedAsync(InventoryId.ToList());
                return RedirectToAction("Index", "Account", new { id = UserName.ToString() });
            }
            public async Task<IActionResult> SearchByTag(string tag)
            {
                var rsp = await _inventoryService.TrySearchByTagAsync(tag);
                return rsp.Count != 0
                    ? View("Search", rsp)
                    : RedirectToAction("Index", "Home");
            }
        }
}


