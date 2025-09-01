using Course_Project.DataAccess.Data;
using Course_Project.DataAccess.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Repostories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<string>> GetAllTagsNamesAcync()
        {
           return await _context.Tags.Select(t=>t.Name).ToListAsync();
        }
        public async Task InsertNewTagsAsync(List<Tag> tags)
        {
            await _context.AddRangeAsync(tags);
            await SaveAllAsync();
        }
        public async Task<List<Tag>> GetAllTagsAcync()
        {
            return await _context.Tags.ToListAsync();
        }
        public List<Tag> GetAllTagsByNames(List<string> Names)
        {
            return _context.Tags.Where(t => Names.Contains(t.Name)).ToList();
        }
        public async Task InsertInventoryTagsAsync(List<InventoryTag> tags)
        {
            await _context.AddRangeAsync(tags);
            await SaveAllAsync();
        }
        public async Task DeleteInventoryTagsAsync(List<InventoryTag> tags)
        {
            _context.RemoveRange(tags);
            await SaveAllAsync();
        }
        public async Task SaveAllAsync() =>
        await _context.SaveChangesAsync();
    }
}
