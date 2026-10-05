namespace FamilyCart.Core.DTOs
{
    public class ProductResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}