using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using System.Diagnostics;
using System.IO;

namespace LifeLine.HrPanel.Desktop.Services.GeneratePdf
{
    public sealed class GeneratePdfService(IFileStorageService fileStorageService) : IGeneratePdfService
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public async Task<byte[]?> GenerateAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine($"[GeneratePdfService] [GenerateAsync] URL пуст!");
                return null;
            }

            var (bucketName, fileName) = S3UrlParser.Parse(url);

            if (string.IsNullOrWhiteSpace(bucketName) && string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"[GeneratePdfService] [GenerateAsync] BucketName и FileName пусты!");
                return null;
            }

            var s3Result = await _fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName!, fileName!));

            if (s3Result.IsFailure && s3Result.Value == null && string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
            {
                Debug.WriteLine($"[GeneratePdfService] [GenerateAsync] S3Result пуст!");
                return null;
            }

            var presginedUrl = s3Result.Value!.PresignedUrl;

            var pdf = await FileHelper.BytesFromUrlAsync(presginedUrl);

            return pdf;
        }

        public async Task<Stream?> GenerateAsStreamAsync(string? url)
        {
            var bytes = await GenerateAsync(url);
            return FileHelper.ToStream(bytes);
        }
    }
}
