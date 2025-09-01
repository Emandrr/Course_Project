using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
using Course_Project.Web.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
namespace Course_Project.Web.ViewModels
{
    public class InventoryEditViewModel
    {
        public Inventory inventory { get; set; }

        public List<string>? Ids { get; set; }

        public List<User>? Users { get; set; }
        public List<string>? UserAccess { get; set; }
        public string? Photo { get; set; }
        public List<Category>? Categories { get; set; }
        public List<string>? AllTags { get; set; }
        public List<string>? InvTags { get; set; }
        public string? Tags { get; set; }
        public int SelectedCategoryId { get; set; }
        public string? ReturnUrl { get; set; }

        [Display(Name = "Photo")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new[] { ".jpg", ".png", ".jpeg" })]
        [MaxFileSize(12 * 1024 * 1024)]
        public IFormFile? ImageFile { get; set; }
    }
}
