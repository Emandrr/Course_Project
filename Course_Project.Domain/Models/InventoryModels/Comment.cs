using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Domain.Models.InventoryModels
{
    public class Comment
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorName { get;set; }
        public Guid PublicId { get; set; }
    }
}
