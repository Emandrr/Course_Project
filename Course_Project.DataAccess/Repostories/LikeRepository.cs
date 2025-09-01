using Course_Project.DataAccess.Data;
using Course_Project.DataAccess.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Repostories
{
    public class LikeRepository :ILikeRepository
    {
        private readonly ApplicationDbContext _context;
        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SetLike(Like like)
        {
            await _context.Likes.AddAsync(like);
            await SaveAllAsync();
        }
        public async Task SaveAllAsync() =>
         await _context.SaveChangesAsync();

        public async Task RemoveLike(Like like)
        {
            _context.Likes.Remove(like);
            await SaveAllAsync();
        }
    }
}
