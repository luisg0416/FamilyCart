namespace FamilyCart.Core.DTOs
{
    public class UpdateProductDto
    {
        public required string? Name { get; set; }
        public required decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}