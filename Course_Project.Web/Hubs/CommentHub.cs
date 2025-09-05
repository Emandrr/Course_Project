using Course_Project.Application.Interfaces;
using Course_Project.Domain.Models.InventoryModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
namespace Course_Project.Web.Hubs
{
    public class CommentHub : Hub
    {
        private readonly ICommentService _commentService;

        public CommentHub(ICommentService commentService)
        {
            _commentService = commentService;
        }
        public async Task AddComment(string content, string userName,Guid inventoryId)
        {
            Comment comm = await _commentService.CreateCommentAsync(inventoryId,userName,content);
            await Clients.All.SendAsync("ReceiveComment", comm);
        }
        public async Task DeleteComments(Guid inventoryId,Guid[] commentId)
        {
            await _commentService.DeleteSelectedCommentsAsync(inventoryId,commentId);
            await Clients.All.SendAsync("RemoveComments", inventoryId,commentId);
        }
    }
}
