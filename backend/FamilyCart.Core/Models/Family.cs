namespace FamilyCart.Core.Models {
    public class Family
    {
        public int Id { get; set; }
        public required User CreatedByUser { get; set; }
        public required List<FamilyMember> Members { get; set; }
        public required string FamilyName { get; set; }
        public required int CreatedById { get; set; } // References User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}