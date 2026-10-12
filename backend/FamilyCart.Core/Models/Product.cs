namespace FamilyCart.Core.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public Family? Family { get; set; }
        public required Store Store { get; set; }
        public required int FamilyId { get; set; } // References Family; every product belongs to a family
        public required int StoreId { get; set; } // References Store
        public List<ListItem> ListItems { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}