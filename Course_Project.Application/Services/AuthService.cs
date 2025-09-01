using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Course_Project.Application.Contracts.DTO;
using Course_Project.Application.Interfaces;
using Course_Project.Domain.Models.UserModels;

namespace Course_Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }
        public async Task<SignInResult> LoginAsync(string Email, string Password)
        {
            User user = await _userManager.FindByEmailAsync(Email);
            if (user == null) return SignInResult.Failed;
            var result = await _signInManager.PasswordSignInAsync(user.UserName, Password, false, false);
            return result;
        }
        public async Task<IdentityResult> RegisterAsync(string Email,string Login,string Password)
        {
            User user = _userService.Create(Email,Login);
            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded) await FillWithPasswordAsync(user);
            return result;
        }
        public async Task<SignInResult> LoginWithExternalAsync(ClaimsPrincipal? result)
        {
            (string, string) ParseResult = await ParseFromGoogle(result);
            UserRes userRes = await RegisterWithExternalAsync(ParseResult);
            if(!userRes.res) return SignInResult.Failed;
            await _signInManager.SignInAsync(userRes.user,true);
            return SignInResult.Success;
        }

        public async Task<UserRes> RegisterWithExternalAsync((string,string)ParseRes)
        {
            User user = await _userManager.FindByEmailAsync(ParseRes.Item1);
            if (user == null) user = await FillWithoutPasswordAsync(ParseRes);
            UserRes result = new UserRes() { user = user};
            if (user.Login != ParseRes.Item2) result.res = false;
            return result;
        }
        public async Task<User> FillWithoutPasswordAsync((string, string) ParseRes)
        {
            User user = await _userService.CreateWithoutPasswordAsync(ParseRes);
            await _userManager.AddToRoleAsync(user, "User");
            return user;
        }
        public async Task FillWithPasswordAsync(User user)
        {
            await _signInManager.SignInAsync(user, true);
            await _userManager.AddToRoleAsync(user, "User");
        }
        public async Task<(string,string)>ParseFromGoogle(ClaimsPrincipal? result)
        {
            string email = result.FindFirstValue(ClaimTypes.Email);
            string login = result.FindFirstValue(ClaimTypes.Name);
            return (email, login);
        }
        public async Task LogoutAsync() 
        {
            await _signInManager.SignOutAsync();
        }
    }
}
