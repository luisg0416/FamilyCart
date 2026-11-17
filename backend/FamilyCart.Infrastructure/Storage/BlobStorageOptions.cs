namespace FamilyCart.Infrastructure.Storage
{
    using System.ComponentModel.DataAnnotations;
    public class BlobStorageOptions
    {
        public const string SectionName = "BlobStorage";
        [Required] public string ConnectionString { get; set; } = string.Empty;
        [Required] public string ContainerName { get; set; } = string.Empty;
        [Range(1, 1440)] public int SasExpiryMinutes { get; set; } = 240;
    }
}