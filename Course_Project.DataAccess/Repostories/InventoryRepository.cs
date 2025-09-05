using Course_Project.DataAccess.Data;
using Course_Project.DataAccess.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace Course_Project.DataAccess.Repostories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Inventory>> GetInventoriesByIdAsync(string UserId)
        {
            return await _context.Inventories.Where(p => p.UserId == UserId).Include(p => p.User).ToListAsync();
        }
        public async Task<List<Inventory>> GetInventoryAccessByIdAsync(string UserId)
        {
            return await _context.Inventories.Where(inv => (inv.UserAccesses.Any(ua => ua.UserId == UserId) || inv.IsPublic) && inv.UserId!=UserId).ToListAsync(); 
        }
        public async Task CreateAsync(Inventory inventory)
        {
            var val = await _context.Inventories.AddAsync(inventory);
            await SaveAllAsync();
        }
        public async Task<Inventory?> GetInventoryByIdAsync(Guid id)
        {
            return await _context.Inventories.Where(p=>p.PublicId==id)
                                  .Include(p=>p.CustomElems)
                                  .Include(p=>p.UserAccesses)
                                  .Include(p=>p.CustomSetOfIds)
                                  .Include(p => p.User)
                                  .Include(i => i.InventoryTags)
                                                       .ThenInclude(it => it.Tag)
                                  .FirstOrDefaultAsync();
        }
        public async Task<Inventory?> GetInventoryByIdAsNoTrackingAsync(Guid id)
        {
            return await _context.Inventories.Where(p => p.PublicId == id)
                                  .Include(p => p.CustomElems)
                                  .Include(p => p.UserAccesses).OrderBy(x => x.Id)
                                  .Include(p => p.CustomSetOfIds).OrderBy(x => x.Id)
                                  .Include(p => p.User)
                                  .Include(p=>p.Items)
                                                      .ThenInclude(item => item.Likes)
                                  .Include(p=>p.Category)
                                  .Include(i => i.InventoryTags)
                                                       .ThenInclude(it => it.Tag)
                                  .Include(i=>i.Comments)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync();
        }
        public async Task SaveAllAsync() =>
                  await _context.SaveChangesAsync();
        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _context.Inventories.Include(p=>p.User).ToListAsync();
        }
        public async Task<List<string>> GetUserAcessAsync(Guid id) 
        {
            return await _context.Inventories
                                 .Where(i => i.PublicId == id)
                                 .SelectMany(i => i.UserAccesses.Select(ua => ua.UserId))
                                 .ToListAsync();
        }
        public async Task<HashSet<string>> GetUserHashAcessAsync(Guid id)
        {
            return await _context.Inventories
                                 .Where(i => i.PublicId == id)
                                 .SelectMany(i => i.UserAccesses.Select(ua => ua.UserId))
                                 .ToHashSetAsync();
        }
        public async Task UpdateAsync(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
            await SaveAllAsync();
        }
        public async Task GiveAccessAsync(List<string> UserIds,Guid inventoryId)
        {
            var accesses = await GetUserHashAcessAsync(inventoryId);
            var UsersToAdd = UserIds.Where(userId => !accesses.Contains(userId))
                                    .Distinct()
                                    .ToList();
            if (UsersToAdd.Any()) await UpdateAccessAsync(UsersToAdd,inventoryId);
        }
        public async Task TakeAccessAsync(List<string> UserIds, Guid inventoryId)
        {
            var rsp = await _context.Inventories.Where(p => p.PublicId == inventoryId).FirstOrDefaultAsync();
            await _context.UserInventoryAccess
                          .Where(ua => ua.InventoryId == rsp.Id && UserIds.Contains(ua.UserId))
                          .ExecuteDeleteAsync();
            await SaveAllAsync();
        }
        public async Task UpdateAccessAsync(List<string> UsersToAdd,Guid inventoryId)
        {
            var rsp = await _context.Inventories.Where(p=>p.PublicId==inventoryId).FirstOrDefaultAsync();
            var newAccesses = UsersToAdd.Select(userId => new UserInventoryAccess
            {
                UserId = userId,
                InventoryId = rsp.Id
            }).ToList();
            await _context.UserInventoryAccess.AddRangeAsync(newAccesses);
            await SaveAllAsync();
        }
        public async Task DeleteAsync(Inventory inventory)
        {
            _context.Inventories.Remove(inventory);
            await SaveAllAsync();
        }
        public async Task<List<Inventory>> TryGetByNameAsync(string Name)
        {
            return await _context.Inventories.Where(inv=>inv.Title ==Name).Include(inv=>inv.User).ToListAsync();
        }
        public async Task<List<Inventory>> TryGetByTagAsync(string tagName)
        {
            return await _context.Inventories
                .Where(inv => inv.InventoryTags.Any(it => it.Tag.Name == tagName)).Include(inv => inv.User)
                .ToListAsync();
        }
        public async Task<List<Inventory>> GetRecentAsync(int count)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.User)
                .Include(i=>i.Items)
                .OrderByDescending(i => i.CreationDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Inventory>> GetTopByItemsAsync(int count)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.User) 
                .Include(i => i.Items)
                .OrderByDescending(i => i.Items.Count)
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<Inventory>> GetInventoryOnGuidsAsync(List<Guid> ids)
        {
            return await _context.Inventories.Where(c => ids.Contains(c.PublicId)).ToListAsync();
        }
        public async Task DeleteItemsAsync(List<Inventory> invs)
        {
            _context.Inventories.RemoveRange(invs);
            await SaveAllAsync();
        }

    }
}
