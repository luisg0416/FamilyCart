using Microsoft.AspNetCore.Identity;
namespace FamilyCart.Core.Models
{
    public class User : IdentityUser <int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}