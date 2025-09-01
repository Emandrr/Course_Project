using Course_Project.Domain.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.InventoryModels
{
    public class InventoryTag
    {
        // This class supposed to solve many-to-many problem for User<->Invetory Model
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}

