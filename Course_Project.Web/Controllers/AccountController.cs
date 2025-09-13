using Course_Project.Application.Interfaces;
using Course_Project.Application.Services;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Course_Project.Web.Controllers
{
    [Authorize(Policy = "AdminOrOwner")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IInventoryService _inventoryService;
        private readonly ICloudService _cloudService;
        public AccountController(IUserService userService, IInventoryService inventoryService, ICloudService cloudService)
        {
            _userService = userService;
            _inventoryService = inventoryService;
            _cloudService = cloudService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSalesforceAccount(string name, SalesforceAccountViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var answer = await _userService.AuthToSalesforceAsync(name,model.CompanyName,model.ContactFirstName,model.ContactLastName,model.ContactEmail);
            if (answer)
                TempData["Success"] = "Account и Contact успешно созданы в Salesforce!";
            else
                TempData["Error"] = "Ошибка при создании в Salesforce.";
            return RedirectToAction("Index","Account" ,new { id=name });
        }
        public async Task<IActionResult> Index(string id)
        {
            var response = await _userService.GetOneByNameAsync(id);
            if(response!=null)return View(new AccountViewModel() { user = response,
                Photo = await _cloudService.GetPhotoAsync(response.PhotoLink),
                UserInventories = await _inventoryService.GetInventoriesAsync(response.Id),
                EditInventories = await _inventoryService.GetInventoriesAccessAsync(response.Id)
            });
            else return View();
        }
    }
}
