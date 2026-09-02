namespace FamilyCart.Core.DTOs
{
    public class CreateShoppingListDto
    {
        public required string ListName { get; set; }
        public required int FamilyId { get; set; }
        public required int StoreId { get; set; }
    }
}