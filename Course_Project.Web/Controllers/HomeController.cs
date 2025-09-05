using Course_Project.Application.Interfaces;
using Course_Project.Application.Services;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Course_Project.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly ITagService _tagService;

    public HomeController(ILogger<HomeController> logger,IInventoryService inventoryService,ITagService tagService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
        _tagService = tagService;
    }

    public async Task<IActionResult> Index()
    {
        var recent = await _inventoryService.GetRecentAsync(5);
        var topByItems = await _inventoryService.GetTopByItemsAsync(5);
        return View(new HomeViewModel
        {
            RecentInventories = recent,
            TopByItemsInventories = topByItems,
            AllTags = await _tagService.GetAllTagsNamesAcync()
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpPost]
    public IActionResult SetLanguage(string returnUrl, string queryStr,string lang)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }
        );

        return LocalRedirect(returnUrl + queryStr);
    }
    [HttpPost]
    public IActionResult ChangeTheme(string returnUrl, string queryStr)
    {
        string theme = Request.Cookies["theme"];
        if(theme==null)Response.Cookies.Delete("theme");
        Response.Cookies.Append("theme", theme == "light" ? "dark" : "light", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1), IsEssential = true }); 
        return LocalRedirect(returnUrl + queryStr);
    }
    public IActionResult RedirectToUser()
    {
        TempData["UserId"] = User.Identity.Name.Split('%')[0];
        return RedirectToAction("Index","Account");
    }
}
