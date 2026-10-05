namespace FamilyCart.Core.DTOs
{
    public class StoreResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int FamilyId { get; set; }
    }
}