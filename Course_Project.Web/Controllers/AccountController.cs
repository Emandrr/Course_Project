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
