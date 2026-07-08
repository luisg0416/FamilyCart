namespace FamilyCart.Core.Models
{
    public class ListItem
    {
        public int Id { get; set; }
        public required ShoppingList ShoppingList { get; set; }
        public required Product Product { get; set; }
        public required User AddedByUser { get; set; }
        public required int ShoppingListId { get; set; } // References ShoppingList
        public required int ProductId { get; set; } // References Product
        public required int Quantity { get; set; }
        public required bool IsChecked { get; set; }
        public required int AddedById { get; set; } // References User
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}