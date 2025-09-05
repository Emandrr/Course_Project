using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course_Project.Application.Contracts.DTO
{
    public class SaveInventory
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SelectedCategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? InvTags { get; set; }
    }
}
