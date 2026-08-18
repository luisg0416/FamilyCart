namespace FamilyCart.Core.DTOs
{
    public class TokenResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        
    }
}