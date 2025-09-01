using System.ComponentModel.DataAnnotations;
using Course_Project.Web.ValidationAttributes;
using Course_Project.Domain.Models.CustomElemsModels;
using Course_Project.Domain.Models.InventoryModels;
namespace Course_Project.Web.ViewModels
{
    public class InventoryCreateViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty; 
        public bool IsPublic { get; set; } = false;
        public string UserId { get; set; }
        public List<Category>? Categories { get; set; }
        public List<string>? AllTags { get; set; }
        public string? Tags { get; set; }
        public int SelectedCategoryId { get; set; }

        [Display(Name = "Photo")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new[] { ".jpg", ".png", ".jpeg" })]
        [MaxFileSize(12 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }
        public List<CustomFieldOption> CustomElems { get; set; } = new List<CustomFieldOption>();

    }
}
