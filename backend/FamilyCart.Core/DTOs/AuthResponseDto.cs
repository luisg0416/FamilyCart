namespace FamilyCart.Core.DTOs
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        public required int UserId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
    }
}
