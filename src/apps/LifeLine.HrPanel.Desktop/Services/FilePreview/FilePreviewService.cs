using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace LifeLine.HrPanel.Desktop.Services.FilePreview
{
    public sealed class FilePreviewService(IFileStorageService fileStorageService, HttpClient httpClient) : IFilePreviewService
    {
        /// <summary>
        /// Скачивает удаленный файл по PresignedUrl и сохраняет во временных файлах
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<string?> DownloadRemoteFileToTempAsync(string url, string fileName)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"[FilePreviewService] [DownloadRemoteFileToTempAsync] URL и FileName пусты!");
                return null;
            }

            var (bucketName, objectPath) = S3UrlParser.Parse(url);

            if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(objectPath))
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateAsync] BucketName и FileName пусты!");
                return null;
            }

            var s3Result = await fileStorageService.GetPresignedUrlAsync(new PresignedUrlRequest(bucketName!, objectPath!));

            if (s3Result.IsFailure && s3Result.Value == null || string.IsNullOrWhiteSpace(s3Result.Value?.PresignedUrl))
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateAsync] S3Result пуст!");
                return null;
            }

            var presignedUrl = s3Result.Value!.PresignedUrl;

            var fileBytes = await httpClient.GetByteArrayAsync(presignedUrl);

            if (fileBytes == null || fileBytes?.Length == 0)
            {
                Debug.WriteLine($"[GenerateImageService] [GenerateAsync] FileBytes пуст!");
                return null;
            }

            var tempPath = await FileHelper.SaveToTempFileAsync(fileBytes, fileName);

            return tempPath;
        }

        /// <summary>
        /// Копирует локальный файл во временные файлы для безопасного открытия
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string? CopyLocalFileToTempAsync(string sourcePath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) && string.IsNullOrWhiteSpace(fileName))
            {
                Debug.WriteLine($"[FilePreviewService] [CopyLocalFileToTempAsync] SourcePath и FileName пусты!");
                return null;
            }

            try
            {
                var safeFileName = Path.GetFileName(fileName);
                var tempPath = Path.Combine(Path.GetTempPath(), safeFileName);

                if (System.IO.File.Exists(tempPath))
                {
                    try
                    {
                        System.IO.File.Delete(tempPath);
                    }
                    catch (Exception)
                    {
                        var ext = Path.GetExtension(safeFileName);
                        var nameWithoutExt = Path.GetFileNameWithoutExtension(safeFileName);

                        tempPath = Path.Combine(Path.GetTempPath(), $"{nameWithoutExt}_{Guid.NewGuid():N:8}{ext}");
                    }
                }

                System.IO.File.Copy(sourcePath, tempPath, overwrite: true);
                return tempPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FilePreviewService] [CopyLocalFileToTempAsync] Ошибка копирования '{sourcePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Открытие файла в приложении по умолчанию
        /// </summary>
        /// <param name="filePath"></param>
        public void OpenInDefaultApplication(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Debug.WriteLine($"[FilePreviewService] [OpenInDefaultApplication] FilePath пусты!");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FilePreviewService] [OpenInDefaultApplication] Ошибка открытия '{filePath}': {ex.Message}");
            }
        }
    }
}
