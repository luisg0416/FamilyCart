namespace FamilyCart.Core.DTOs
{
    public class CreateListItemDto
    {
        public required int ShoppingListId { get; set; }
        public required int ProductId { get; set; }
        public required int Quantity { get; set; }
    }
}