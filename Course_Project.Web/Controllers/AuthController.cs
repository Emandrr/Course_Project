using Course_Project.Application.Interfaces;
using Course_Project.Domain.Models;
using Course_Project.Web.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Course_Project.Web.Controllers
{
    public class AuthController:Controller
    {
        private readonly IAuthService _authService;
        private readonly string EmailProblem = "DuplicateEmail";
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [Authorize(Policy = "NotAuthenticated")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (TempData.TryGetValue("AuthError", out object error) && error is string errorMessage)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
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
                else if (!answer.Succeeded) ModelState.AddModelError(string.Empty, "Invalid email and (or) password");
                if (!answer.Succeeded) return View(model);
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
        [Authorize(Policy = "NotAuthenticated")]
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse"),
                    Items = { { "prompt", "select_account" } }
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var res = await Login();
            if (res.Succeeded) return RedirectToAction("Index", "Home",new { area = "" });
            else
            {
                TempData["AuthError"] = "Login with Google failed";
                return RedirectToAction("Login", "Auth");
            }
        }
        [Authorize(Policy = "NotAuthenticated")]
        public IActionResult LinkedInLogin()
        {
            var redirectUri = Url.Action("LinkedinResponse", "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
                Items =
        {
            { "prompt", "login" }
        }
            };
            return Challenge(properties, "LinkedIn");
        }
        private async Task<Microsoft.AspNetCore.Identity.SignInResult> Login()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return await _authService.LoginWithExternalAsync(result.Principal);
        }
        public async Task<IActionResult> LinkedInResponse()
        {
            var res = await Login();
            if (res.Succeeded) return RedirectToAction("Index", "Home");
            else
            {
                TempData["AuthError"] = "Login with Linkedin failed";
                return RedirectToAction("Login", "Auth");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
