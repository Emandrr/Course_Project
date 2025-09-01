using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.DataAccess.Interfaces
{
    public interface ITagRepository
    {
        public Task<List<string>> GetAllTagsNamesAcync();
        public Task<List<Tag>> GetAllTagsAcync();
        public Task SaveAllAsync();
        public Task InsertNewTagsAsync(List<Tag> tags);
        public List<Tag> GetAllTagsByNames(List<string> Names);
        public Task InsertInventoryTagsAsync(List<InventoryTag> tags);
        public Task DeleteInventoryTagsAsync(List<InventoryTag> tags);
    }
}
