using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.CustomElemsModels
{
    public class CustomField
    {
        public int Id { get; set; }  
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public CustomFieldType FieldType { get; set; }
        public bool IsVisible { get; set; }
    }
}
