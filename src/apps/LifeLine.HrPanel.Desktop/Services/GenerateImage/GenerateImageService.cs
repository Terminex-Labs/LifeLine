using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using System.Windows.Media;

namespace LifeLine.HrPanel.Desktop.Services.GenerateImage
{
    public sealed class GenerateImageService(IFileStorageService fileStorageService) : IGenerateImageService
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public async Task<ImageSource?> GenerateAsync(string? personalPhoto)
        {
            if (string.IsNullOrWhiteSpace(personalPhoto))
                return null;

            var (bucketName, fileName) = S3UrlParser.Parse(personalPhoto);

            var s3Result = await _fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName!, fileName!));

            if (s3Result.IsFailure || s3Result.Value == null)
                return null;

            var presignedUrl = s3Result.Value.PresignedUrl;

            var image = await ImageHelper.ImageFromUrlAsync(presignedUrl);

            return image;
        }

        public async Task<byte[]?> GenerateBytesAsync(string? personalPhoto)
        {
            if (string.IsNullOrWhiteSpace(personalPhoto))
                return null;

            var (bucketName, fileName) = S3UrlParser.Parse(personalPhoto);
            var s3Result = await _fileStorageService.GetPresignedUrlAsync(
                new PresignedUrlRequest(bucketName!, fileName!));

            if (s3Result.IsFailure || string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
                return null;

            return await ImageHelper.BytesFromUrlAsync(s3Result.Value.PresignedUrl);
        }
    }
}
