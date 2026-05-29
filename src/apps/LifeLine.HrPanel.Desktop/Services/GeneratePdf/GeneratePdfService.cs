using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using System.IO;

namespace LifeLine.HrPanel.Desktop.Services.GeneratePdf
{
    public sealed class GeneratePdfService(IFileStorageService fileStorageService) : IGeneratePdfService
    {
        private readonly IFileStorageService _fileStorageService = fileStorageService;

        public async Task<byte[]?> GenerateAsync(string? s3Url)
        {
            if (string.IsNullOrWhiteSpace(s3Url))
                return null;

            var (bucketName, fileName) = S3UrlParser.Parse(s3Url);

            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
                return null;

            var s3Result = await _fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName, fileName));

            if (s3Result.IsFailure || string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
                return null;
            
            var pdf = await PdfHelper.BytesFromUrlAsync(s3Result.Value.PresignedUrl);

            return pdf;
        }

        public async Task<Stream?> GenerateAsStreamAsync(string? s3Url)
        {
            var bytes = await GenerateAsync(s3Url);
            return bytes == null ? null : new MemoryStream(bytes);
        }
    }
}
