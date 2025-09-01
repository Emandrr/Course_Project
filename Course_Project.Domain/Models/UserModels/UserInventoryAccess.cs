using Course_Project.Domain.Models.InventoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.UserModels
{
    // This class supposed to solve many-to-many problem for User<->Invetory Model
    public class UserInventoryAccess
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
