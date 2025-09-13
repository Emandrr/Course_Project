using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface IUserService
    {
        public User Create(string Email, string Login);
        public Task<User> CreateWithoutPasswordAsync((string, string) ParseRes);  
        public Task<List<User>> GetAllUsersAsync();
        public Task UnblockSelectedAsync(string[] UserId); 
        public Task BlockSelectedAsync(string[] UserId);
        public Task GiveAdminSelectedAsync(string[] UserId);
        public Task TakeAdminSelectedAsync(string[] UserId);
        public Task BlockAsync(User user);
        public Task UnBlockAsync(User user);
        public Task<bool> DeleteAllSelectedAsync(string[] UserId);
        public Task<bool> DeleteSelectedNotSelfAsync(string SelectedId);
        public Task<User?> GetOneByNameAsync(string? name);
        public Task<bool> AuthToSalesforceAsync(string name, string CompanyName, string ContactFirstName, string ContactLastName, string ContactEmail);
    }
}
