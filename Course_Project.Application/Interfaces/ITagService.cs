using Course_Project.Application.Services;
using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface ITagService
    {
        public Task<List<string>> GetAllTagsNamesAcync();
        public Task<List<string>> GetAllTagsNamesForInventoryAsync(string id);
        public Task InsertNewTagsAsync(List<string> ProbablyNewTags);
        public Task TryAddTagsToInventory(string GuidId, List<string> ProbablyNewTags);
        public Task AddTagsToInventoryAsync(int id, List<string> ProbablyNewTags, string GuidId);
        public Task UpdateTagsForInventoryAsync(string GuidId, List<string> ProbablyNewTags);
        public Task TryDeleteTagsToInvenotory(string GuidId);
    }
}
