using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface ICommentRepository
    {
        public Task AddCommentAsync(Comment comment);
        public Task SaveAllAsync();
        public Task DeleteCommentAsync(Comment comment);
        public Task DeleteCommentsAsync(List<Comment> comments);
        public Task<List<Comment>> GetCommentsAsync(List<Guid> guids);
    }
}
