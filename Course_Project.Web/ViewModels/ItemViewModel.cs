using Course_Project.Domain.Models.InventoryModels;

namespace Course_Project.Web.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        public bool HasChance { get; set; } = false;
        public bool IsSet { get; set; }
        public bool IsEdit { get; set; }
        public string? Photo { get; set; }
        public Inventory? Inventory { get; set; }
    }
}
