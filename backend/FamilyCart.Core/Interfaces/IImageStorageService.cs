namespace FamilyCart.Core.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(Stream stream, string contentType, string blobName, CancellationToken cancellationToken = default);
        Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);
        Task<string?> GetReadUrlAsync(string? blobName, CancellationToken cancellationToken = default);
    }
}