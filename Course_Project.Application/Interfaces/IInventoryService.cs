using Course_Project.Application.Contracts.DTO;
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
    public interface IInventoryService
    {
        public Task<Guid> CreateAsync(string Title, string Description, bool IsPublic, List<CustomIdRule> CustomSetOfIds, List<Item> Items, List<CustomFieldOption> CustomElems, IFormFile file, string UserId,int categoryId);
        public Inventory Set(string Title, string Description, bool IsPublic, List<CustomIdRule> CustomSetOfIds, List<Item> Items, List<CustomFieldOption> CustomElems, IFormFile file, string UserId,Category category);
        public Task<Inventory?> GetInventoryAsync(string id);
        public Task<List<Inventory>> GetAllAsync();
        public Task<bool> CanEdit(Inventory inventory, string UserId);
        public Task UpdateAsync(Inventory inventory);
        public Task UpdateCustomElemsAsync(List<CustomElem> customElems, Inventory inventory);
        public void SetCustomElems(List<CustomElem> customElems, List<CustomFieldOption> customFields);
        public Task UpdateCustomSetOfIdsAsync(List<CustomSetOfId> customSetOfIds, Inventory inventory);
        public void SetCustomSetOfIds(List<CustomSetOfId> customSetOfIds, List<CustomIdRule> customIdRules);
        public Task<List<string>> GetAccess(Guid id);
        public Task GiveAccessSelectedAsync(string[] UserId, Guid inventoryId);
        public Task TakeAccessSelectedAsync(string[] UserId, Guid inventoryId);
        public Task DeleteAsync(Inventory inventory);
        public void Reset(string Name, string Description, IFormFile file, int categoryId,Inventory inventory);
        public Task UpdateMainFieldsAsync(string Name, string Description, IFormFile file, int categoryId,Inventory inventory);
        public Task<List<Inventory>> GetInventoriesAsync(string id);
        public Task<List<Inventory>> GetInventoriesAccessAsync(string id);
        public Task<List<Inventory>?> TrySearchAsync(string query);
        public Task<Inventory?> GetInventoryAsNoTrackingAsync(string id);
        public Task<List<Inventory>> GetRecentAsync(int count);
        public Task<List<Inventory>> GetTopByItemsAsync(int count);
        public Task<List<Inventory>> TrySearchByTagAsync(string tag);
        public Task DeleteSelectedAsync(List<Guid> guids);
    }
}
