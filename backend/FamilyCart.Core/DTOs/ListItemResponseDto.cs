namespace FamilyCart.Core.DTOs
{
    public class ListItemResponseDto
    {
        public required int Id { get; set; }
        public required int ShoppingListId { get; set; }
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
        public required bool IsChecked { get; set; }
        public required int AddedById { get; set; }
        public required DateTime AddedAt { get; set; }
    }
}