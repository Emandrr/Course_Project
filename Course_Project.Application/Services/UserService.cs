using Course_Project.Application.Interfaces;
using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Course_Project.Application.Services
{
    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        public UserService(UserManager<User> userManager) 
        {
            _userManager = userManager;
        }

        public User Create(string Email,string Login)
        {
            return new User
            {
                Login = Login,
                Email = Email,
                UserName = Login,
                LockoutEnabled =false
            };
        }
        public async Task<User> CreateWithoutPasswordAsync((string,string)ParseRes)
        {
            User newUser = Create(ParseRes.Item1, ParseRes.Item2);
            await _userManager.CreateAsync(newUser);
            return newUser;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.AsNoTracking().ToListAsync();
        }
        public async Task UnblockSelectedAsync(string[] UserId)
        {
            foreach (var id in UserId)
            {
                User? user = await _userManager.FindByIdAsync(id);
                await UnBlockAsync(user);
            }
        }
        public async Task BlockSelectedAsync(string[] UserId)
        {
            foreach (var id in UserId)
            {
                User? user = await _userManager.FindByIdAsync(id);
                await BlockAsync(user);
            }
        }
        public async Task GiveAdminSelectedAsync(string[] UserId)
        {
            foreach (var id in UserId)
            {
                User? user = await _userManager.FindByIdAsync(id);
                await _userManager.AddToRoleAsync(user,"Admin");
                await _userManager.RemoveFromRoleAsync(user, "User");
                user.IsRoleChanged = true;
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task TakeAdminSelectedAsync(string[] UserId)
        {
            foreach (var id in UserId)
            {
                User? user = await _userManager.FindByIdAsync(id);
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.RemoveFromRoleAsync(user,"Admin");
                user.IsRoleChanged = true;  
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task BlockAsync(User user)
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(200);
            user.LockoutEnabled = true;
            user.IsLocked = true;
            var result = await _userManager.UpdateAsync(user);
        }
        public async Task UnBlockAsync(User user)
        {
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow;
                user.LockoutEnabled = false;
                user.IsLocked = false;
                var result = await _userManager.UpdateAsync(user);
            }
        }
        public async Task<bool> DeleteAllSelectedAsync(string[] UserId)
        {
            bool value = false;
            foreach (var id in UserId)
            {
                value = await DeleteSelectedNotSelfAsync(id);
            }
            return value;
        }
        public async Task<bool> DeleteSelectedNotSelfAsync(string SelectedId)
        {
            User? user = await _userManager.FindByIdAsync(SelectedId);
            if (user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _userManager.DeleteAsync(user);
                return false;
            }
            return true;
        }
        public async Task<User?> GetOneByNameAsync(string? name)
        {
            return await _userManager.Users.Where(p=>p.UserName==name).AsNoTracking().FirstOrDefaultAsync();   
        }
    }
}
