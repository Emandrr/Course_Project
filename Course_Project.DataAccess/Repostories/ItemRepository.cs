using Course_Project.DataAccess.Data;
using Course_Project.DataAccess.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Course_Project.DataAccess.Repostories
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;
        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items.Where(i=>i.Id==id).
                                        Include(i=>i.Inventory).
                                        Include(i=>i.CustomFields).
                                        Include(i=>i.CustomId).
                                        AsNoTracking().
                                        Include(i=>i.Likes).FirstOrDefaultAsync();
        }
        public async Task SaveAllAsync() =>
         await _context.SaveChangesAsync();
        public async Task CreateAsync(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

        }
        public async Task<Item?> GetItemByIdAsync(Guid id)
        {
            return await _context.Items.Where(p => p.PublicId == id)
                                  .Include(p => p.Inventory)
                                  .Include(p => p.CustomFields)
                                  .Include(p => p.Likes)
                                  .FirstOrDefaultAsync();
        }
        public async Task<Item?> GetItemByIdAsNoTrackingAsync(Guid id)
        {
            return await _context.Items.Where(p => p.PublicId == id)
                                  .Include(p => p.Inventory)
                                  .Include(p => p.CustomFields)
                                  .Include(p => p.Likes)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync();
        }
        public async Task UpdateAsync(Item item)
        {
            _context.Items.Update(item);
            await SaveAllAsync();
        }
        public async Task<List<string>> GetUserLikesAsync(Guid id)
        {
            return await _context.Items
                                 .Where(i => i.PublicId == id)
                                 .SelectMany(i => i.Likes.Select(ua => ua.UserId))
                                 .ToListAsync();
        }
        public async Task<Item?> GetItemByCustomIdAsync(string customid)
        {
            return await _context.Items.Where(i => i.CustomIdWithInventoryId == customid).AsNoTracking().FirstOrDefaultAsync();
        }
        public async Task<List<Item>> GetItemOnGuidsAsync(List<Guid> ids)
        {
            return await _context.Items.Where(c => ids.Contains(c.PublicId)).ToListAsync();
        }
        public async Task DeleteItemAsync(Item item)
        {
            _context.Items.Remove(item);
            await SaveAllAsync();
        }
        public async Task DeleteItemsAsync(List<Item> items)
        {
            _context.Items.RemoveRange(items);
            await SaveAllAsync();
        }
    }
}
