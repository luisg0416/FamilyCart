namespace FamilyCart.Core.DTOs
{
    public class CreateProductDto
    {
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
    }
}