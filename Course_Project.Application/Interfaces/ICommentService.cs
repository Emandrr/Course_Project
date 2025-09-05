using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface ICommentService
    {
        public Task DeleteSelectedCommentsAsync(Guid id,Guid[] CommentIds);
        public Task<Comment> CreateCommentAsync(Guid InventoryId, string AuthorName, string Description);

    }
}
