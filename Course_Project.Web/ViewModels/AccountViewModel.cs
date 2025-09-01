using Course_Project.Domain.Models.InventoryModels;
using Course_Project.Domain.Models.UserModels;
namespace Course_Project.Web.ViewModels
{
    public class AccountViewModel
    {
        public User user { get; set; }

        public List<Inventory> EditInventories { get; set; } = new List<Inventory>();
        public List<Inventory> UserInventories { get; set; } = new List<Inventory>();
    }
}
