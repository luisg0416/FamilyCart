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
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public ProductType Type { get; set; }
        public string? InstacartProductId { get; set; }
        public Family? Family { get; set; }
        public Store Store { get; set; }
        public int? FamilyId { get; set; }
        public int StoreId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}