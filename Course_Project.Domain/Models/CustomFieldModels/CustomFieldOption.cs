using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.CustomElemsModels
{
    public class CustomFieldOption
    {
        public int Id { get; set; }
        public int? InventoryId { get; set; }
        public Inventory? Inventory { get; set; }
        public string Name { get; set; }
        public CustomFieldType FieldType { get; set; }
        public bool? IsVisible { get; set; } = false;
        public string? Description { get; set; }
    }
}
