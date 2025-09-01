using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface IItemRepository
    {
        public Task<Item?> GetItemByIdAsync(int id);
        public Task SaveAllAsync();
        public Task CreateAsync(Item item);
        public Task UpdateAsync(Item item);
        public Task<Item?> GetItemByIdAsNoTrackingAsync(Guid id);
        public Task<Item?> GetItemByIdAsync(Guid id);
        public Task<List<string>> GetUserLikesAsync(Guid id);
    }
}
