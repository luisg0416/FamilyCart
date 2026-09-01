namespace FamilyCart.Core.DTOs
{
    public class StoreResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? InstacartStoreId { get; set; }
        public int? FamilyId { get; set; }
    }
}