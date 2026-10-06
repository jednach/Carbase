using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace Carbase.Services
{
    public class CarImageService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;
        private const int MinWidth = 100;
        private const int MinHeight = 100;
        private const int MaxWidth = 6000;
        private const int MaxHeight = 6000;

        private static readonly HashSet<string> AllowedExtensions =
        [
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ];

        private static readonly HashSet<string> AllowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];

        private readonly IWebHostEnvironment _environment;

        public CarImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            ValidateBasicFileInfo(file);

            await using var inputStream = file.OpenReadStream();

            ImageInfo? imageInfo;

            try
            {
                imageInfo = await Image.IdentifyAsync(inputStream);
            }
            catch (UnknownImageFormatException)
            {
                throw new InvalidOperationException(
                    "The uploaded file is not a valid image.");
            }
            catch (InvalidImageContentException)
            {
                throw new InvalidOperationException(
                    "The uploaded image is corrupted or invalid.");
            }

            if (imageInfo is null)
            {
                throw new InvalidOperationException(
                    "The uploaded file is not a valid image.");
            }

            var detectedFormat = imageInfo.Metadata.DecodedImageFormat;

            var extension = detectedFormat switch
            {
                JpegFormat => ".jpg",
                PngFormat => ".png",
                WebpFormat => ".webp",
                _ => throw new InvalidOperationException(
                    "Only JPG, PNG and WEBP images are allowed.")
            };

            if (imageInfo.Width < MinWidth ||
                imageInfo.Height < MinHeight)
            {
                throw new InvalidOperationException(
                    $"Image dimensions must be at least {MinWidth}x{MinHeight} pixels.");
            }

            if (imageInfo.Width > MaxWidth ||
                imageInfo.Height > MaxHeight)
            {
                throw new InvalidOperationException(
                    $"Image dimensions cannot exceed {MaxWidth}x{MaxHeight} pixels.");
            }

            inputStream.Position = 0;

            Image image;

            try
            {
                image = await Image.LoadAsync(inputStream);
            }
            catch (UnknownImageFormatException)
            {
                throw new InvalidOperationException(
                    "The uploaded file is not a valid image.");
            }
            catch (InvalidImageContentException)
            {
                throw new InvalidOperationException(
                    "The uploaded image is corrupted or invalid.");
            }

            using (image)
            {
                image.Metadata.ExifProfile = null;
                image.Metadata.IccProfile = null;
                image.Metadata.XmpProfile = null;
                image.Metadata.IptcProfile = null;

                var fileName = $"{Guid.NewGuid()}{extension}";

                var directory = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "cars");

                Directory.CreateDirectory(directory);

                var filePath = Path.Combine(
                    directory,
                    fileName);

                await SaveImageAsync(
                    image,
                    filePath,
                    extension);

                return $"/uploads/cars/{fileName}";
            }
        }

        public void DeleteFile(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            var relativePath = imagePath
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var fullPath = Path.Combine(
                _environment.WebRootPath,
                relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        private static void ValidateBasicFileInfo(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new InvalidOperationException(
                    "Image file is empty.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new InvalidOperationException(
                    "Image cannot be larger than 5 MB.");
            }

            var extension = Path
                .GetExtension(file.FileName)
                .ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Only JPG, PNG and WEBP images are allowed.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                throw new InvalidOperationException(
                    "Invalid image content type.");
            }
        }

        private static async Task SaveImageAsync(
            Image image,
            string filePath,
            string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    await image.SaveAsync(
                        filePath,
                        new JpegEncoder());

                    break;

                case ".png":
                    await image.SaveAsync(
                        filePath,
                        new PngEncoder());

                    break;

                case ".webp":
                    await image.SaveAsync(
                        filePath,
                        new WebpEncoder());

                    break;

                default:
                    throw new InvalidOperationException(
                        "Unsupported image format.");
            }
        }
    }
}