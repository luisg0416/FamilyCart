namespace FamilyCart.Core.DTOs
{
    public class ShoppingListDetailResponseDto
    {
        public required int Id { get; set; }
        public required string ListName { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required List<ListItemDetailResponseDto> ListItems { get; set; }
    }
}