using Course_Project.Application.Services;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Course_Project.Domain.Models;
namespace Course_Project.Web.Controllers
{
    public class AuthController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AuthService _authService;
        private readonly string EmailProblem = "DuplicateEmail";
        public AuthController(UserManager<User>userManager,SignInManager<User> signInManager,AuthService authService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _authService = authService;
        }
        [Authorize(Policy = "NotAuthenticated")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [Authorize(Policy = "NotAuthenticated")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var answer = await _authService.LoginAsync(model.Email,model.Password);
                if (answer.IsLockedOut) ModelState.AddModelError(string.Empty, "You are blocked");
                else if (!answer.Succeeded) ModelState.AddModelError(string.Empty, "Invalid login and (or) password");
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return Redirect(model.ReturnUrl);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        [Authorize(Policy = "NotAuthenticated")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Authorize(Policy = "NotAuthenticated")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var answer = await _authService.RegisterAsync(model.Email,model.Login,model.Password);
                if (answer.Succeeded) return RedirectToAction("Index", "Home");
                foreach (var error in answer.Errors)
                {
                    if (error.Code == EmailProblem) ModelState.AddModelError(string.Empty, "This  email is already taken");
                }
            }
            return View(model);
        }
    }
}
