using Course_Project.Application.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace Course_Project.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IInventoryService _inventoryService;

    public HomeController(ILogger<HomeController> logger,IInventoryService inventoryService)
    {
        _logger = logger;
        _inventoryService = inventoryService;
    }

    public IActionResult Index()
    {

        return View();
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
