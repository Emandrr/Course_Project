using Course_Project.Domain.Models.InventoryModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface IInventoryRepository
    {
        public Task<List<Inventory>> GetInventoriesByIdAsync(string UserId);

        public Task CreateAsync(Inventory inventory);
        public Task SaveAllAsync();
        public Task<Inventory?> GetInventoryByIdAsync(Guid id);
        public Task<List<Inventory>> GetAllAsync();
        public Task<List<string>> GetUserAcessAsync(Guid id);
        public Task UpdateAsync(Inventory inventory);
        public Task GiveAccessAsync(List<string> UserIds, Guid inventoryId);
        public Task TakeAccessAsync(List<string> UserIds, Guid inventoryId);
        public Task UpdateAccessAsync(List<string> UsersToAdd, Guid inventoryId);
        public Task DeleteAsync(Inventory inventory);
        public Task<List<Inventory>> GetInventoryAccessByIdAsync(string UserId);
        public Task<List<Inventory>> TryGetByNameAsync(string Name);
        public Task<Inventory?> GetInventoryByIdAsNoTrackingAsync(Guid id);

    }
}
