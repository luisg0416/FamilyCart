namespace FamilyCart.Core.DTOs
{
    public class ListItemDetailResponseDto
    {
        public required int Id { get; set; }
        public required int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public required int Quantity { get; set; }
        public required bool IsChecked { get; set; }
        public required int AddedById { get; set; }
        public required DateTime AddedAt { get; set; }
        public required bool IsArchived { get; set; }
        
    }
}