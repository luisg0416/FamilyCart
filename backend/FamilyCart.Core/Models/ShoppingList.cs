namespace FamilyCart.Core.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public required string ListName { get; set; }
        public required Family Family { get; set; }
        public required Store Store { get; set; }
        public required User CreatedByUser { get; set; }
        public required int FamilyId { get; set; } // References Family
        public required int StoreId { get; set; } // References Store
        public required int CreatedById { get; set; } // References User
        public List<ListItem> ListItems { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}