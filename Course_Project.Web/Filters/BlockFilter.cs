using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Course_Project.Web.Filters
{
    public class BlockFilter : IAsyncResourceFilter
    {
        private readonly string IdentityCookie = ".AspNetCore.Identity.Application";


        UserManager<User> _userManager;
        SignInManager<User> _signinManager;
        public BlockFilter(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signinManager = signInManager;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (context.HttpContext.Request.RouteValues["controller"]?.ToString() == "Inventory" && context.HttpContext.Request.RouteValues["action"]?.ToString() == "Details" && !Guid.TryParse(context.HttpContext.Request.RouteValues["id"].ToString(), out Guid res)) return;
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = await _userManager.FindByIdAsync(_userManager.GetUserId(context.HttpContext.User));
                var currentUrl = context.HttpContext.Request.Path;
                if (result != null)
                {
                    if (result.IsLocked && currentUrl != "/Auth/Login" || result.IsRoleChanged)
                    {
                        await Clear(context);
                        if (result.IsRoleChanged) await Insure(result);
                    }
                }
                else await Clear(context);
            }
            await next();
        }
        public async Task Clear(ResourceExecutingContext context)
        {
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            await _signinManager.SignOutAsync();
        }
        public async Task Insure(User result)
        {
            result.IsRoleChanged = false;
            await _signinManager.SignInAsync(result, false);
            await _userManager.UpdateAsync(result);
        }

    }
}
