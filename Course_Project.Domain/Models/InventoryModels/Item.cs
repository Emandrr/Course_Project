using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.CustomIdModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.InventoryModels
{
    public class Item
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatorName { get; set; }
        public string CustomId { get; set; }
        public List<CustomField> CustomFields { get;set; } = new List<CustomField>();
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public List<Like> Likes { get; set; } = new List<Like>();
        public string? PhotoLink { get; set; }
    }
}
