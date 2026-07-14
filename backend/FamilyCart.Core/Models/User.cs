using Microsoft.AspNetCore.Identity;
namespace FamilyCart.Core.Models
{
    public class User : IdentityUser <int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public List<ShoppingList> ShoppingLists { get; set; } = new(); 
        public List<ListItem> ListItems { get; set; } = new();
        public List<Family> FamiliesCreated { get; set; } = new(); 
        public List<FamilyMember> FamilyMemberships { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}