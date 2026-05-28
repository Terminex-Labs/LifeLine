using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.Files;
using Shared.Kernel.Errors;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Terminex.Common.Results;

namespace LifeLine.File.Service.Client
{
    internal class FileStorageService(HttpClient httpClient) : IFileStorageService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<Result<PresignedUrlResponse?>> GetPresignedUrlAsync(PresignedUrlRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("presigned-url", request, _jsonSerializerOptions);

                if (!response.IsSuccessStatusCode)
                    return Result<PresignedUrlResponse?>.Failure(Error.New(ErrorCode.NotFound, await response.Content.ReadAsStringAsync()));

                return Result<PresignedUrlResponse?>.Success(await response.Content.ReadFromJsonAsync<PresignedUrlResponse>());
            }
            catch (Exception ex)
            {
                return Result<PresignedUrlResponse?>.Failure(Error.New(ErrorCode.NotFound, $"Ошибка получения: {ex.Message}"));
            }
        }

        public async Task<Result<GetFileMetadataResponse?>> GetFileMetadataAsync(GetFileMetadataRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("files/meta-data", request, _jsonSerializerOptions);

                if (!response.IsSuccessStatusCode)
                    return Result<GetFileMetadataResponse?>.Failure(Error.New(ErrorCode.NotFound, await response.Content.ReadAsStringAsync()));

                return Result<GetFileMetadataResponse?>.Success(await response.Content.ReadFromJsonAsync<GetFileMetadataResponse>());
            }
            catch (Exception ex)
            {
                return Result<GetFileMetadataResponse?>.Failure(Error.New(ErrorCode.NotFound, $"Ошибка получения: {ex.Message}"));
            }
        }

        public async Task<Result> DeleteFileAsync(DeleteFileRequest request)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/files/{request.BucketName}/{request.FileName}");

                if (!response.IsSuccessStatusCode)
                    return Result.Failure(Error.New(ErrorCode.NotFound, await response.Content.ReadAsStringAsync()));

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(Error.New(ErrorCode.NotFound, $"Ошибка получения: {ex.Message}"));
            }
        }

        public async Task<Result<UploadFileResponse?>> UploadFileAsync(UploadFileRequest request)
        {
            using var formData = new MultipartFormDataContent
                {
                    { new StringContent(request.BucketName), nameof(request.BucketName) },
                    { new StringContent(request.AdditionalName), nameof(request.AdditionalName) }
                };

            if (!string.IsNullOrWhiteSpace(request.SubFolder))
                formData.Add(new StringContent(request.SubFolder, Encoding.UTF8, "text/plain"), nameof(request.SubFolder));

            StreamContent? streamContent = null;
            string? fileName = null;
            string? mimeType = null;

            if (request.FileBytes != null && !string.IsNullOrWhiteSpace(request.FileName))
            {
                var ms = new MemoryStream(request.FileBytes);
                streamContent = new StreamContent(ms);
                fileName = request.FileName;
                mimeType = request.ContentType ?? "application/pdf";
            }
            else if (!string.IsNullOrWhiteSpace(request.FilePath) && System.IO.File.Exists(request.FilePath))
            {
                var fs = System.IO.File.OpenRead(request.FilePath);
                streamContent = new StreamContent(fs);
                fileName = Path.GetFileName(request.FilePath);
                mimeType = GetMimeType(request.FilePath);
            }

            if (streamContent == null)
                return Result<UploadFileResponse?>.Failure(Error.New(AppErrors.Upload, "Файл не указан или не найден!"));

            streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType!);
            formData.Add(streamContent, "File", fileName!);

            try
            {
                var response = await _httpClient.PostAsync($"files", formData);
                response.EnsureSuccessStatusCode();

                return Result<UploadFileResponse?>.Success(await response.Content.ReadFromJsonAsync<UploadFileResponse?>());
            }
            catch (Exception ex)
            {
                return Result<UploadFileResponse?>.Failure(Error.New(AppErrors.Upload, $"Ошибка загрузки изображения!\n{ex.Message}"));
            }
        }

        public async Task<Result<List<UploadFileResponse>?>> UploadFilesAsync(UploadFilesRequest request)
        {
            using var formData = new MultipartFormDataContent();

            for (int i = 0; i < request.Files.Count; i++)
            {
                var file = request.Files[i];
                var prefix = $"Files[{i}]";

                formData.Add(new StringContent(file.BucketName), $"{prefix}.BucketName");
                formData.Add(new StringContent(file.AdditionalName), $"{prefix}.AdditionalName");

                if (!string.IsNullOrWhiteSpace(file.SubFolder))
                    formData.Add(new StringContent(file.SubFolder, Encoding.UTF8, "text/plain"), $"{prefix}.SubFolder");

                StreamContent? streamContent = null;
                string? fileName = null;
                string? mimeType = null;

                if (file.FileBytes != null && !string.IsNullOrWhiteSpace(file.FileName))
                {
                    var ms = new MemoryStream(file.FileBytes);
                    streamContent = new StreamContent(ms);
                    fileName = file.FileName;
                    mimeType = file.ContentType ?? "application/pdf";
                } 
                else if (!string.IsNullOrWhiteSpace(file.FilePath) && System.IO.File.Exists(file.FilePath))
                {
                    var fs = System.IO.File.OpenRead(file.FilePath);
                    streamContent = new StreamContent(fs);
                    fileName = Path.GetFileName(file.FilePath);
                    mimeType = GetMimeType(file.FilePath);
                }

                if (streamContent != null)
                {
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType!);
                    formData.Add(streamContent, $"{prefix}.File", fileName!);
                }
            }

            try
            {
                var response = await _httpClient.PostAsync("files/batch", formData);
                response.EnsureSuccessStatusCode();

                return Result<List<UploadFileResponse>?>.Success(await response.Content.ReadFromJsonAsync<List<UploadFileResponse>>());
            }
            catch (Exception ex)
            {
                return Result<List<UploadFileResponse>?>.Failure(Error.New(AppErrors.Upload, $"Ошибка загрузки!\n{ex.Message}"));
            }
        }

        public async Task<string> GetLink(string key)
        {
            var encodeKey = Uri.EscapeDataString(key);

            var response = await _httpClient.GetAsync($"api/files/link?key={encodeKey}");

            return await response.Content.ReadAsStringAsync();
        }

        private readonly Dictionary<string, string> MimeMappings = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".bmp", "image/bmp" },
            { ".svg", "image/svg+xml" },
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
        };

        public string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (extension == null || !MimeMappings.TryGetValue(extension, out var mimeType))
                return "application/octet-stream";

            return mimeType;
        }
    }
}
