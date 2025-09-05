using Course_Project.Domain.Models.InventoryModels;

namespace Course_Project.Web.ViewModels
{
    public class HomeViewModel
    {
        public List<Inventory> RecentInventories { get; set; } = new List<Inventory>();
        public List<Inventory> TopByItemsInventories { get; set; } = new List<Inventory>();
        public List<string> AllTags { get; set; }
    }
}
