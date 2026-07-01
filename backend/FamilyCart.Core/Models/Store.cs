namespace FamilyCart.Core.Models {
    public class Store {
        public int Id { get; set; } 
        public required string Name { get; set; }
        public string? InstacartStoreId { get; set; }
    }
}