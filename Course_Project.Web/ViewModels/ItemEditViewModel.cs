using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Course_Project.Web.ViewModels
{
    public class ItemEditViewModel
    {
        public string Name { get; set; }
        public string CreatorName { get; set; }
        public List<CustomFieldInputViewModel> CustomFields { get; set; } = new List<CustomFieldInputViewModel>();
        public Inventory? Inventory { get; set; }
        public List<string>? Ids { get; set; }
        public string CustomIdString { get; set; }
        [Display(Name = "Photo")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new[] { ".jpg", ".png", ".jpeg" })]
        [MaxFileSize(12 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }
        public Item Item { get; set; }
        public bool HasChance { get; set; } = false;
        public bool IsSet { get; set; }
        public string? Photo { get; set; }
    }
}
