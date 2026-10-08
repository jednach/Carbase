using Amazon.S3.Model;

namespace Carbase.Services.Storage
{
    public interface ICarImageStorage
    {
        Task<string> SaveAsync(
        Stream imageStream,
        string fileName,
        string contentType);

        Task DeleteAsync(string? imagePath);

        Task<GetObjectResponse> GetAsync(string fileName);
    }
}
