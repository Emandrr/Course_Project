using Course_Project.Application.Interfaces;
using Course_Project.Domain.Models;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Course_Project.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController:Controller
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task <IActionResult> Index()
        {
            return View(await _userService.GetAllUsersAsync());
        }
        public async Task<IActionResult> Block(string[] UserId)
        {
            await _userService.BlockSelectedAsync(UserId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Unblock(string[] UserId)
        {
            await _userService.UnblockSelectedAsync(UserId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string[] UserId)
        {
            await _userService.DeleteAllSelectedAsync(UserId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GiveAdmin(string[] UserId)
        {
            await _userService.GiveAdminSelectedAsync(UserId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> TakeAdmin(string[] UserId)
        {
            await _userService.TakeAdminSelectedAsync(UserId);
            return RedirectToAction("Index");
        }
    }
}
