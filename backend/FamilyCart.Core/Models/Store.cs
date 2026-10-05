namespace FamilyCart.Core.Models {
    public class Store {
        public int Id { get; set; }
        public required string Name { get; set; }
        public Family? Family { get; set; }
        public required int FamilyId { get; set; } // Every store belongs to a family
        public List<ShoppingList> ShoppingLists { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}