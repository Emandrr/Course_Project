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
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await SaveAllAsync();
        }
        public async Task SaveAllAsync()=>
            await _context.SaveChangesAsync();
        public async Task DeleteCommentAsync(Comment comment) 
        {
            _context.Comments.Remove(comment);
            await SaveAllAsync();
        }
        public async Task DeleteCommentsAsync(List<Comment> comments)
        {
            _context.Comments.RemoveRange(comments);
            await SaveAllAsync();
        }
        public async Task<List<Comment>> GetCommentsAsync(List<Guid> guids)
        {
            return await _context.Comments.Where(c => guids.Contains(c.PublicId)).ToListAsync();
        }
    }
}
