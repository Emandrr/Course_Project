using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Course_Project.Domain.Models;
namespace Course_Project.Application.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly UserService _userService;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }
        public async Task<SignInResult> LoginAsync(string Email, string Password)
        {
            User user = await _userManager.FindByIdAsync(Email);
            if (user == null) return SignInResult.Failed;
            var result = await _signInManager.PasswordSignInAsync(user.UserName, Password, false, false);
            return result;
        }
        public async Task<IdentityResult> RegisterAsync(string Email,string Login,string Password)
        {
            User user = _userService.Create(Email,Login);
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded) await _signInManager.SignInAsync(user, false);
            return result;
        }
    }
}
