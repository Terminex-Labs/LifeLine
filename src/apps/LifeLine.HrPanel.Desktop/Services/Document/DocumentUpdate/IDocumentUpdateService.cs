using Shared.Contracts.Request.Files;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentUpdate
{
    public interface IDocumentUpdateService
    {
        Task<Result> UpdateDocumentFileAsync<TDbRequest>(byte[] newFileBytes, string newFileName, string? oldFileKey, Func<byte[], string, UploadFileRequest> uploadRequest, Func<string, TDbRequest> dbRequest, Func<TDbRequest, Task<Result>> dbUpdateAsync);
    }
}