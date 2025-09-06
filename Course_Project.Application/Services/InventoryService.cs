using Course_Project.Application.Contracts.DTO;
using Course_Project.Application.Interfaces;
using Course_Project.DataAccess.Interfaces;
using Course_Project.DataAccess.Repostories;
using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Course_Project.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ICloudService _cloudService;
        private readonly UserManager<User>_userManager;
        private readonly ICategoryRepository _categoryRepository;
        private readonly string _folderId = "1o_vyB7A01EsYmwT-Aoaj4eaHny_kB1E7";
        private readonly string _fileId= "1-MNB5BJ85Z6fOH01php9t2ym6syx6De4";
        public InventoryService(IInventoryRepository inventoryRepository,ICloudService cloudService,UserManager<User> userManager,ICategoryRepository categoryRepository) 
        {
            _inventoryRepository = inventoryRepository;
            _cloudService = cloudService;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
        }
        public Inventory Set(string Title,string Description,bool IsPublic,List<CustomIdRule> CustomSetOfIds, List<Item> Items, List<CustomFieldOption> CustomElems,IFormFile file,string UserId,Category category)
        {
            return new Inventory()
            {
                Title = Title,
                Description = Description,
                IsPublic = IsPublic,
                CustomElems = CustomElems,
                CustomSetOfIds = CustomSetOfIds,
                Items = Items,
                PhotoLink = file == null ? _fileId : _cloudService.UploadPhoto(_folderId, file),
                UserId = UserId,
                CreationDate = DateTime.UtcNow,
                PublicId = Guid.NewGuid(),
                Category = category,
                Version = 1
            };
        }
        public async Task<Guid> CreateAsync(string Title, string Description, bool IsPublic, List<CustomIdRule> CustomSetOfIds, List<Item> Items, List<CustomFieldOption> CustomElems, IFormFile file, string UserId,int categoryId)
        {
            Inventory NewInventory = Set(Title, Description, IsPublic, CustomSetOfIds, Items, CustomElems, file,UserId,await _categoryRepository.GetCategoryByIdAsync(categoryId));
            await _inventoryRepository.CreateAsync(NewInventory);
            return NewInventory.PublicId;
        }
        public async Task<Inventory?> GetInventoryAsNoTrackingAsync(string id)
        {
            if (Guid.TryParse(id, out Guid res)) return await _inventoryRepository.GetInventoryByIdAsNoTrackingAsync(res);
            else return null;
        }
        public async Task<Inventory?> GetInventoryAsync(string id)
        {
            if (Guid.TryParse(id, out Guid res)) return await _inventoryRepository.GetInventoryByIdAsync(res);
            else return null;
        }
        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _inventoryRepository.GetAllAsync();
        }
        public async Task<List<string>> GetAccess(Guid id)
        {
            return await _inventoryRepository.GetUserAcessAsync(id);
        }
        public async Task<List<Inventory>> GetInventoriesAsync(string id)
        {
            return await _inventoryRepository.GetInventoriesByIdAsync(id);
        }
        public async Task<List<Inventory>> GetInventoriesAccessAsync(string id)
        {
            return await _inventoryRepository.GetInventoryAccessByIdAsync(id);
        }
        public async Task<bool> CanEdit(Inventory inventory,string Name)
        {
            if (Name == null) return false;
            User user = await _userManager.FindByNameAsync(Name);
            List<string> accesses = await _inventoryRepository.GetUserAcessAsync(inventory.PublicId);
            if (inventory.IsPublic || accesses.Contains(user.Id )) return true;
            else return false;
        }
        public async Task UpdateAsync(Inventory inventory)
        {
            await _inventoryRepository.UpdateAsync(inventory);
        }
        public async Task UpdateCustomElemsAsync(List<CustomElem> customElems,Inventory inventory)
        {
            List<CustomFieldOption> customFields = new List<CustomFieldOption>();
            SetCustomElems(customElems, customFields);
            inventory.CustomElems = customFields;
            await _inventoryRepository.UpdateAsync(inventory);
        }
        public void SetCustomElems(List<CustomElem> customElems, List<CustomFieldOption> customFields)
        {
            for (int i = 0; i < customElems.Count; i++)
            {
                customFields.Add(new CustomFieldOption()
                {
                    IsVisible = customElems[i].IsVisible,
                    Name = customElems[i].Name,
                    FieldType = customElems[i].FieldType,
                    Description = customElems[i].Description
                });
            }
        }
        public async Task UpdateCustomSetOfIdsAsync(List<CustomSetOfId> customSetOfIds, Inventory inventory)
        {
            List<CustomIdRule> customIdOptions = new List<CustomIdRule>();
            SetCustomSetOfIds(customSetOfIds, customIdOptions);
            inventory.CustomSetOfIds.Clear();
            foreach (var option in customIdOptions)
            {
                inventory.CustomSetOfIds.Add(option);
            }
            await _inventoryRepository.UpdateAsync(inventory);
        }

        public void SetCustomSetOfIds(List<CustomSetOfId> customSetOfIds, List<CustomIdRule> customIdRules)
        {
            for (int i = 0; i < customSetOfIds.Count; i++)
            {
                if (customSetOfIds[i].IdType == IdType.Rand20Bit || customSetOfIds[i].IdType == IdType.Rand32Bit) customSetOfIds[i].Rule = customSetOfIds[i].Rule == "zeros" ? "zeros" : "no-zeros";
                if (customSetOfIds[i].IdType==IdType.DateTime) customSetOfIds[i].Rule = customSetOfIds[i].Rule == "date" ? "date" : customSetOfIds[i].Rule == "mont"?"month":"year";
                customIdRules.Add(new CustomIdRule()
                {
                    IdType = customSetOfIds[i].IdType,
                    Rule = customSetOfIds[i].Rule
                });
            }
        }
        public async Task GiveAccessSelectedAsync(string[] UserId,Guid inventoryId )
        {
            await _inventoryRepository.GiveAccessAsync(UserId.ToList(),inventoryId);
        }
        public async Task TakeAccessSelectedAsync(string[] UserId, Guid inventoryId)
        {
            await _inventoryRepository.TakeAccessAsync(UserId.ToList(), inventoryId);
        }
        public async Task DeleteAsync(Inventory inventory)
        {
            await _inventoryRepository.DeleteAsync(inventory);
        }
        public void Reset(string Name,string Description,IFormFile file,int categoryId,Inventory inventory)
        {
            inventory.Title = Name;
            inventory.Description = Description;
            inventory.PhotoLink = file == null ? inventory.PhotoLink : _cloudService.UploadPhoto(_folderId, file);
            inventory.CategoryId = categoryId;
        }

        public async Task UpdateMainFieldsAsync(string Name, string Description, IFormFile file, int categoryId,Inventory inventory)
        {
            Reset(Name, Description, file, categoryId,inventory);
            await _inventoryRepository.UpdateAsync(inventory);
        }
        public async Task<List<Inventory>> TrySearchAsync(string query)
        {
            return await _inventoryRepository.TryGetByNameAsync(query);
        }
        public async Task<List<Inventory>> GetRecentAsync(int count)
        {
            return await _inventoryRepository.GetRecentAsync(count);  
        }
        public async Task<List<Inventory>> GetTopByItemsAsync(int count)
        {
            return await _inventoryRepository.GetTopByItemsAsync(count);
        }
        public async Task DeleteSelectedAsync(List<Guid> guids)
        {
            var ans = await _inventoryRepository.GetInventoryOnGuidsAsync(guids.ToList());
            await _inventoryRepository.DeleteItemsAsync(ans);
        }
        public async Task<List<Inventory>> TrySearchByTagAsync(string tag)
        {
            return await _inventoryRepository.TryGetByTagAsync(tag);
        }
    }
}
