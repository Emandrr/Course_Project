using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;

namespace Course_Project.Web.ViewModels
{
    public class InventoryInfoViewModel
    {
        public Inventory inventory { get; set; }
        public string? Photo { get; set; }
        public User? user { get; set; }
        public List<string>? UserAccess { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
