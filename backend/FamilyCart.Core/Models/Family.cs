namespace FamilyCart.Core.Models {
    public class Family
    {
        public int Id { get; set; }
        public string FamilyName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}