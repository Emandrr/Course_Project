using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.InventoryModels
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    }
}
