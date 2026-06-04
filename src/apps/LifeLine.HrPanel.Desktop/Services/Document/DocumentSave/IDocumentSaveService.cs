using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentSave
{
    public interface IDocumentSaveService
    {
        Task<Result> SaveLocalDocumentsAsync<TDisplay, TDbRequest>(IEnumerable<TDisplay> documentsToSave, Func<TDisplay, UploadFilesDataRequest> uploadRequest, Func<TDisplay, string, TDbRequest> dbRequest, Func<IEnumerable<TDbRequest>, Task<Result>> dbSaveAsync, Action<TDisplay> markAsSavedAction, CancellationToken ct = default);
    }
}