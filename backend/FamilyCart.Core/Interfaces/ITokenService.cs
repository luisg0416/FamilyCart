namespace FamilyCart.Core.Interfaces
{
    using FamilyCart.Core.DTOs;
    using FamilyCart.Core.Models;
    public interface ITokenService
    {
        TokenResponseDto GetToken(User user);
    }
}