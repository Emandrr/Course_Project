using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.InventoryModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Interfaces
{
    public interface IItemService
    {
        public Task<Guid> CreateAsync(int InventoryId, string Name, string CreatorName, List<CustomField> CustomFields, string CustomId,IFormFile file);
        public Item Set(int InventoryId, string Name, string CreatorName, List<CustomField> CustomFields, string CustomId,IFormFile file);
        public Task<Item?> GetItemAsNoTrackingAsync(string id);
        public Task<Item> GetItemAsync(string id);
        public Task<bool> HasLikeAsync(string Username, Guid ItemId);
        public Task SetLike(string name, Guid invid);
        public Task RemoveLike(string name, Guid invid);
        public Task<Item?> GetItemByCustomIdAsync(string customid);
        public Task EditItemAsync(string Name, List<CustomField> customFields, string customId, IFormFile file,string ItemId);
        public void ReSet(string Name,List<CustomField> CustomFields, string CustomId, IFormFile file,Item item);
        public Task<bool> CheckOnCustomId(string customId,string PublicItmId, string PublicInvId);
        public Task DeleteSelectedAsync(Guid[] guids);
    }
}
