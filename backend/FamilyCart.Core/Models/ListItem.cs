namespace FamilyCart.Core.Models
{
    public class ListItem
    {
        public int Id { get; set; }
        public required int ShoppingListId { get; set; }
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public required bool IsChecked { get; set; }
        public required int AddedById { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}