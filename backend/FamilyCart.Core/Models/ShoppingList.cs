namespace FamilyCart.Core.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public required string ListName { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
        public required int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}