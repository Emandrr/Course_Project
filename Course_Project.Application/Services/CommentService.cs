using Course_Project.Application.Interfaces;
using Course_Project.DataAccess.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICommentRepository _commentRepository;
        public CommentService(IInventoryRepository inventoryRepository,ICommentRepository commentRepository)
        {
            _inventoryRepository = inventoryRepository;
            _commentRepository = commentRepository;
        }
        public async Task DeleteSelectedCommentsAsync(Guid id, Guid[] CommentIds)
        {
            var ans = await _commentRepository.GetCommentsAsync(CommentIds.ToList());
            await _commentRepository.DeleteCommentsAsync(ans);
        }
        public async  Task<Comment> CreateCommentAsync(Guid InventoryId, string AuthorName, string Description)
        {
            var inv = await _inventoryRepository.GetInventoryByIdAsNoTrackingAsync(InventoryId);
            Comment comment = Set(inv.Id, AuthorName, Description);
            await _commentRepository.AddCommentAsync(comment);
            return comment;
        }
        public Comment Set(int InventoryId, string AuthorName, string Description)
        {
            return new Comment() { AuthorName = AuthorName, 
                Description = Description,
                CreatedDate=DateTime.UtcNow,
                InventoryId = InventoryId,
                PublicId=Guid.NewGuid()};
        }
    }
}
