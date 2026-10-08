using Amazon.S3;
using Amazon.S3.Model;

namespace Carbase.Services.Storage
{
    public class R2CarImageStorage : ICarImageStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public R2CarImageStorage(
            IAmazonS3 s3Client,
            IConfiguration configuration)
        {
            _s3Client = s3Client;

            _bucketName = configuration["R2:BucketName"]
                ?? throw new InvalidOperationException(
                    "R2 bucket name is not configured.");
        }

        public async Task<string> SaveAsync(
            Stream imageStream,
            string fileName,
            string contentType)
        {
            var objectKey = $"cars/{fileName}";

            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                InputStream = imageStream,
                ContentType = contentType,
                AutoCloseStream = false,
                DisablePayloadSigning = true,
                DisableDefaultChecksumValidation = true
            };

            await _s3Client.PutObjectAsync(request);

            return $"/images/cars/{fileName}";
        }

        public async Task DeleteAsync(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            const string prefix = "/images/cars/";

            if (!imagePath.StartsWith(
                    prefix,
                    StringComparison.Ordinal))
            {
                return;
            }

            var fileName = imagePath[prefix.Length..];

            if (string.IsNullOrWhiteSpace(fileName) ||
                fileName.Contains('/') ||
                fileName.Contains('\\'))
            {
                return;
            }

            await _s3Client.DeleteObjectAsync(
                new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"cars/{fileName}"
                });
        }

        public Task<GetObjectResponse> GetAsync(string fileName)
        {
            return _s3Client.GetObjectAsync(
                new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"cars/{fileName}"
                });
        }
    }
}
