namespace FamilyCart.Core.DTOs
{
    public class CreateStoreDto
    {
        public required string Name { get; set; }
        public required int FamilyId { get; set; }
        public string? InstacartStoreId { get; set; }
    }
}