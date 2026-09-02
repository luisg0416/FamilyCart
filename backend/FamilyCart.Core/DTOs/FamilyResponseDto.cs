namespace FamilyCart.Core.DTOs
{
    public class FamilyResponseDto
    {
        public required int Id { get; set; }
        public required string FamilyName { get; set; }
        public required string InviteCode { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}