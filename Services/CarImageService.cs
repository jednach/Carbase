using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Carbase.Services
{
    public class CarImageService
    {
        private const long MaxFileSize = 15 * 1024 * 1024;
        private const int MinWidth = 100;
        private const int MinHeight = 100;
        private const int MaxWidth = 6000;
        private const int MaxHeight = 6000;
        private const int MaxImageDimension = 1920;

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

            _ = detectedFormat switch
            {
                JpegFormat => true,
                PngFormat => true,
                WebpFormat => true,
                _ => throw new InvalidOperationException(
                    "Only JPG, PNG and WEBP images are allowed.")
            };

            if (imageInfo.FrameMetadataCollection.Count > 1)
            {
                throw new InvalidOperationException(
                    "Animated images are not allowed.");
            }

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
                image.Mutate(x => x.AutoOrient());

                if (image.Width > MaxImageDimension ||
                    image.Height > MaxImageDimension)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxImageDimension, MaxImageDimension)
                    }));
                }

                image.Metadata.ExifProfile = null;
                image.Metadata.IccProfile = null;
                image.Metadata.XmpProfile = null;
                image.Metadata.IptcProfile = null;

                var fileName = $"{Guid.NewGuid()}.webp";

                var directory = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "cars");

                Directory.CreateDirectory(directory);

                var filePath = Path.Combine(
                    directory,
                    fileName);

                await image.SaveAsync(filePath, new WebpEncoder
                {
                    Quality = 80,
                    Method = WebpEncodingMethod.Fastest,
                    FileFormat = WebpFileFormatType.Lossy
                });

                return $"/uploads/cars/{fileName}";
            }
        }

        public void DeleteFile(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return;
            }

            var fileName = Path.GetFileName(imagePath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }

            var fullPath = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "cars",
                fileName);

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
                    "Image cannot be larger than 15 MB.");
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
    }
}