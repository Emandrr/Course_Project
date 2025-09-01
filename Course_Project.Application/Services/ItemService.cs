using Course_Project.Application.Interfaces;
using Course_Project.DataAccess.Interfaces;
using Course_Project.DataAccess.Repostories;
using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Services
{
    public class ItemService : IItemService
    {
        
        private readonly IItemRepository _itemRepository; 
        private readonly ILikeRepository _likeRepository;
        private readonly UserManager<User> _userManager;
        private readonly ICloudService _cloudService;
        private readonly string _folderId = "1o_vyB7A01EsYmwT-Aoaj4eaHny_kB1E7";
        private readonly string _fileId = "1-MNB5BJ85Z6fOH01php9t2ym6syx6De4";
        public ItemService(IItemRepository itemRepository,UserManager<User> userManager,ILikeRepository likeRepository,ICloudService cloudService)
        {
            _itemRepository = itemRepository;
            _userManager = userManager;
            _likeRepository = likeRepository;
            _cloudService = cloudService;
        }
        public Item Set(int InventoryId, string Name, string CreatorName, List<CustomField> CustomFields, string CustomId, IFormFile file)
        {
            return new Item() 
            {  
                InventoryId = InventoryId, 
                Name = Name, 
                CreatorName = CreatorName, 
                CustomFields = CustomFields, 
                CustomId = CustomId,
                CreationDate = DateTime.UtcNow,
                PublicId = Guid.NewGuid(),
                PhotoLink = file == null ? _fileId : _cloudService.UploadPhoto(_folderId, file)
            }; 
        }
        public async Task<Guid> CreateAsync(int InventoryId, string Name, string CreatorName, List<CustomField> CustomFields, string CustomId, IFormFile file)
        {
            Item item = Set(InventoryId, Name, CreatorName, CustomFields, CustomId, file);
            await _itemRepository.CreateAsync(item);
            return item.PublicId;
        }
        public async Task<Item?> GetItemAsNoTrackingAsync(string id)
        {
            if (Guid.TryParse(id, out Guid res)) return await _itemRepository.GetItemByIdAsNoTrackingAsync(res);
            else return null;
        }
        public async Task<Item?> GetItemAsync(string id)
        {
            if (Guid.TryParse(id, out Guid res)) return await _itemRepository.GetItemByIdAsync(res);
            else return null;
        }
        public async Task<bool> HasLikeAsync(string Username, Guid ItemId)
        {
            if (Username == null) return false;
            User usr = await _userManager.FindByNameAsync(Username);
            if (usr!=null)
            {
                List<string> UserIds = await _itemRepository.GetUserLikesAsync(ItemId);
                return UserIds.Contains(usr.Id);
            }
            return false;
        
        }
        public async Task SetLike(string name, Guid invid)
        {
            User user = await _userManager.FindByNameAsync(name);
            Item item = await _itemRepository.GetItemByIdAsNoTrackingAsync(invid);
            if (user != null && item != null) await _likeRepository.SetLike(new Like() { ItemId = item.Id, UserId = user.Id });
        }
        public async Task RemoveLike(string name, Guid invid)
        {
            User user = await _userManager.FindByNameAsync(name);
            Item item = await _itemRepository.GetItemByIdAsNoTrackingAsync(invid);
            if (user != null && item != null) await _likeRepository.RemoveLike(new Like() { ItemId=item.Id,UserId=user.Id});
        }

    }
}
