using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using Course_Project.Domain.Models.UserModels;
namespace Course_Project.Domain.Models.InventoryModels
{
    public class Inventory
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
        public string? PhotoLink{ get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<CustomIdRule> CustomSetOfIds{get;set;} = new List<CustomIdRule>();
        public List<Item> Items { get; set; } = new List<Item>();
        public List<CustomFieldOption> CustomElems { get; set; } = new List<CustomFieldOption>();
        public List<UserInventoryAccess> UserAccesses { get; set; } = new List<UserInventoryAccess>();
        public List<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
        public List<Comment> Comments { get; set; } = new List<Comment> { };
        public DateTime CreationDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int Version { get; set; }
    }
}
