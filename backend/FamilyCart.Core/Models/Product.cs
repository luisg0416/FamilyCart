namespace FamilyCart.Core.Models
{
    public enum ProductType
    {
        Instacart,
        Family
    }
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public ProductType Type { get; set; }
        public string? InstacartProductId { get; set; }
        public Family? Family { get; set; }
        public required Store Store { get; set; }
        public int? FamilyId { get; set; } // References Family; can be null
        public required int StoreId { get; set; } // References Store
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}