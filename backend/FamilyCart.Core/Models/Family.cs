namespace FamilyCart.Core.Models {
    public class Family
    {
        public int Id { get; set; }
        public required User CreatedByUser { get; set; }
        public List<FamilyMember> FamilyMembers { get; set; } = new();
        public List <Product> Products { get; set; } = new();
        public List<ShoppingList> ShoppingLists { get; set; } = new();
        public required string FamilyName { get; set; }
        public required int CreatedById { get; set; } // References User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}