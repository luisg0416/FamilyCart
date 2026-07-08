namespace FamilyCart.Core.Models
{
    public class FamilyMember
    {
        public int Id { get; set; }
        public required Family Family { get; set; }
        public required User User { get; set; }
        public required int FamilyId { get; set; } // References Family
        public required int UserId { get; set;} // References User
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}