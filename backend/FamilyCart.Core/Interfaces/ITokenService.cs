namespace FamilyCart.Core.Interfaces
{
    using FamilyCart.Core.Models;
    public interface ITokenService
    {
        string GetToken(User user);
    }
}