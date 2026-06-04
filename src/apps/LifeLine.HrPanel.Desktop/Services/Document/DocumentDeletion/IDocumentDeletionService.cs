using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentDeletion
{
    public interface IDocumentDeletionService
    {
        Task<Result> DeleteDocumentAsync<TDisplay>(TDisplay document, IList<TDisplay> collection, Func<TDisplay, Guid> getId, Func<TDisplay, string?> getFileKey, Func<Guid, Task<Result>> dbDeleteAsync, Action refreshView);
    }
}