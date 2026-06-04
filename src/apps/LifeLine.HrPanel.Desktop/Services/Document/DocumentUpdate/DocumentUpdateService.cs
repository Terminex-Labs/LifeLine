using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentUpdate
{
    public sealed class DocumentUpdateService(IFileStorageService storageService) : IDocumentUpdateService
    {
        public async Task<Result> UpdateDocumentFileAsync<TDbRequest>
            (
                byte[] newFileBytes,
                string newFileName,
                string? oldFileKey,
                Func<byte[], string, UploadFileRequest> uploadRequest,
                Func<string, TDbRequest> dbRequest,
                Func<TDbRequest, Task<Result>> dbUpdateAsync
            )
        {
            var s3Request = uploadRequest(newFileBytes, newFileName);
            var s3Result = await storageService.UploadFileAsync(s3Request);

            if (s3Result.IsFailure)
                return Result.Failure(Error.BadRequest(s3Result.StringMessage));

            var s3FileName = s3Result.Value!.FileName;

            var dbReq = dbRequest(s3FileName);
            var dbResult = await dbUpdateAsync(dbReq);

            if (dbResult.IsFailure)
                return Result.Failure(Error.BadRequest(dbResult.StringMessage));

            if (!string.IsNullOrWhiteSpace(oldFileKey))
            {
                var (bucketName, fileName) = S3UrlParser.Parse(oldFileKey);

                await storageService.DeleteFileAsync(new DeleteFileRequest(bucketName!, fileName!));
            }

            return Result.Success();
        }
    }
}
