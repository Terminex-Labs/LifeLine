using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;
using Shared.WPF.Helpers;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentDeletion
{
    public sealed class DocumentDeletionService(IFileStorageService storageService) : IDocumentDeletionService
    {
        public async Task<Result> DeleteDocumentAsync<TDisplay>
            (
                TDisplay document,
                IList<TDisplay> collection,
                Func<TDisplay, Guid> getId,
                Func<TDisplay, string?> getFileKey,
                Func<Guid, Task<Result>> dbDeleteAsync,
                Action refreshView
            )
        {
            if (document == null)
                return Result.Failure(Error.Validation("Документ для удаления не указан!"));

            List<Error> errors = [];

            var fileKey = getFileKey(document);

            if (string.IsNullOrWhiteSpace(fileKey))
            {
                collection.Remove(document);
                refreshView();
                return Result.Success();
            }

            var documentId = getId(document);

            var dbResult = await dbDeleteAsync(documentId);

            if (dbResult.IsFailure)
                errors.AddRange(dbResult.Errors);

            var (bucketName, fileName) = S3UrlParser.Parse(fileKey);

            var deleteFileResult = await storageService.DeleteFileAsync(new DeleteFileRequest(bucketName!, fileName!));

            if (deleteFileResult.IsFailure)
                errors.AddRange(deleteFileResult.Errors);

            collection.Remove(document);
            refreshView();

            return errors.Any() ? Result.Failure(errors) : Result.Success();
        }
    }
}
