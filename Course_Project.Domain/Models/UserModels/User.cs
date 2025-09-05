
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Course_Project.Domain.Models.InventoryModels;
using Microsoft.AspNetCore.Identity;
namespace Course_Project.Domain.Models.UserModels
{
    public class User : IdentityUser
    {
        public string Login { get; set; } = string.Empty;
        public bool IsLocked { get; set; } = false;
        public bool IsRoleChanged { get; set; } = false;
        public List<Inventory>UserInventories{get;set;}=new List<Inventory>();
        public List<UserInventoryAccess> InventoryAccesses { get; set; } = new List<UserInventoryAccess>();
        public List<Like> Likes { get; set; } = new List<Like>();
        public string? PhotoLink { get; set; }
    }
}
