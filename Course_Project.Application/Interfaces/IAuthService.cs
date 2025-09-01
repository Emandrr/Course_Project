using Course_Project.Application.Contracts.DTO;
using Course_Project.Application.Services;
using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<SignInResult> LoginAsync(string Email, string Password);
        public Task<IdentityResult> RegisterAsync(string Email, string Login, string Password);
        public Task<SignInResult> LoginWithExternalAsync(ClaimsPrincipal? result);

        public Task<UserRes> RegisterWithExternalAsync((string, string) ParseRes);
        public Task<User> FillWithoutPasswordAsync((string, string) ParseRes);
        public Task FillWithPasswordAsync(User user);
        public Task<(string, string)> ParseFromGoogle(ClaimsPrincipal? result);
        public Task LogoutAsync();
    }
}
