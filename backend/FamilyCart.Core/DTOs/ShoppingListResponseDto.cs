namespace FamilyCart.Core.DTOs
{
    public class ShoppingListResponseDto
    {
        public required int Id { get; set; }
        public required string ListName { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }

    }
}