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
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private IInventoryService _inventoryService;
        public TagService(ITagRepository tagRepository, IInventoryRepository inventoryRepository,IInventoryService inventoryService)
        {
            _tagRepository = tagRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryService = inventoryService;
        }
        public async Task<List<string>> GetAllTagsNamesAcync()
        {
            return await _tagRepository.GetAllTagsNamesAcync(); 
        }
        public async Task<List<string>> GetAllTagsNamesForInventoryAsync(string id)
        {
            Inventory inv = await _inventoryService.GetInventoryAsNoTrackingAsync(id);
            if(inv!=null)return inv.InventoryTags.Select(it => it.Tag.Name).ToList();
            return new List<string>();
        }
        public async Task InsertNewTagsAsync(List<string> ProbablyNewTags)
        {
            List<string> AllTags = await GetAllTagsNamesAcync();
            List<string> NewTags = ProbablyNewTags.Except(AllTags).ToList();
            List<Tag> TagsToInsert = NewTags.Select(name => new Tag { Name = name }).ToList();
            await _tagRepository.InsertNewTagsAsync(TagsToInsert);
        }
        public async Task TryAddTagsToInventory(string GuidId, List<string> ProbablyNewTags)
        {
            await InsertNewTagsAsync(ProbablyNewTags);
            Inventory inv = await _inventoryService.GetInventoryAsync(GuidId);
            if (inv != null) await AddTagsToInventoryAsync(inv.Id, ProbablyNewTags,GuidId);

        }
        public async Task AddTagsToInventoryAsync(int id, List<string> ProbablyNewTags,string GuidId)
        {
            List<Tag> TagsToAdd = _tagRepository.GetAllTagsByNames(ProbablyNewTags);
            List<InventoryTag> inventoryTags = TagsToAdd.Select(tag => new InventoryTag { TagId=tag.Id,Tag=tag,InventoryId=id}).ToList();
            await _tagRepository.InsertInventoryTagsAsync(inventoryTags);
        }
        public async Task UpdateTagsForInventoryAsync(string GuidId, List<string> ProbablyNewTags)
        {
            await TryDeleteTagsToInvenotory(GuidId);
            await TryAddTagsToInventory(GuidId, ProbablyNewTags);
        }
        public async Task TryDeleteTagsToInvenotory(string GuidId)
        {
            Inventory inv = await _inventoryService.GetInventoryAsync(GuidId);
            if (inv != null) await _tagRepository.DeleteInventoryTagsAsync(inv.InventoryTags);
        }
    }
}
