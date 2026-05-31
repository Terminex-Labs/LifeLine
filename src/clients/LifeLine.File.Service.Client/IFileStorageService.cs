using Shared.Contracts.Request.Files;
using Shared.Contracts.Response.Files;
using Terminex.Common.Results;

namespace LifeLine.File.Service.Client
{
    public interface IFileStorageService
    {
        Task<Result<PresignedUrlResponse?>> GetPresignedUrlAsync(PresignedUrlRequest request);
        Task<Result<GetFileMetadataResponse?>> GetFileMetadataAsync(GetFileMetadataRequest request);
        Task<Result> DeleteFileAsync(DeleteFileRequest request);
        Task<Result<UploadFileResponse?>> UploadFileAsync(UploadFileRequest request);
        Task<Result<List<UploadFileResponse>?>> UploadFilesAsync(UploadFilesRequest request);
        Task<string> GetLink(string key);
    }
}
