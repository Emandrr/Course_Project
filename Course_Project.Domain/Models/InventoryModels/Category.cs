using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.InventoryModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Inventory> Inventories { get; set; }

    }
}
