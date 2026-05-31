using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using System.Diagnostics;
using System.Windows.Media;

namespace LifeLine.HrPanel.Desktop.Services.GenerateImage
{
    public sealed class GenerateImageService(IFileStorageService fileStorageService) : IGenerateImageService
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public async Task<ImageSource?> GenerateAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine($"\n[GenerateImageService] [GenerateAsync] URL пуст!\n");
                return null;
            }

            var (bucketName, fileName) = S3UrlParser.Parse(url);

            if (string.IsNullOrWhiteSpace(bucketName) && string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"\n[GenerateImageService] [GenerateAsync] BucketName и FileName пусты!\n");
                return null;
            }

            var s3Result = await _fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName!, fileName!));

            if (s3Result.IsFailure && s3Result.Value == null && string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
            {
                Debug.WriteLine($"\n[GenerateImageService] [GenerateAsync] S3Result пуст!\n");
                return null;
            }

            var presignedUrl = s3Result.Value!.PresignedUrl;

            var image = await FileHelper.ImageFromUrlAsync(presignedUrl);

            return image;
        }

        public async Task<byte[]?> GenerateBytesAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateBytesAsync] URL пуст!");
                return null;
            }

            var (bucketName, fileName) = S3UrlParser.Parse(url);

            if (string.IsNullOrWhiteSpace(bucketName) && string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateBytesAsync] BucketName и FileName пусты!");
                return null;
            }

            var s3Result = await _fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName!, fileName!));

            if (s3Result.IsFailure && s3Result.Value == null && string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateBytesAsync] S3Result пуст!");
                return null;
            }

            var presignedUrl = s3Result.Value!.PresignedUrl;

            var bytes = await FileHelper.BytesFromUrlAsync(presignedUrl);

            return bytes;
        }
    }
}
