using Terminex.Common.Results;
using LifeLine.File.Service.Client;
using Shared.Contracts.Request.Files;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentSave
{
    public sealed class DocumentSaveService(IFileStorageService storageService) : IDocumentSaveService
    {
        public async Task<Result> SaveLocalDocumentsAsync<TDisplay, TDbRequest>
            (
                IEnumerable<TDisplay> documentsToSave,
                Func<TDisplay, UploadFilesDataRequest> uploadRequest,
                Func<TDisplay, string, TDbRequest> dbRequest,
                Func<IEnumerable<TDbRequest>, Task<Result>> dbSaveAsync,
                Action<TDisplay> markAsSavedAction,
                CancellationToken ct = default
            )
        {
            if (documentsToSave == null || !documentsToSave.Any())
                return Result.Success();

            // Формирование запроса для s3-хранилища
            var uploadRequests = documentsToSave.Select(uploadRequest).ToList();

            // Отправка в s3-хранилище
            var uploadResult = await storageService.UploadFilesAsync(new UploadFilesRequest(uploadRequests));

            if (uploadResult.IsFailure)
                return Result.Failure(Error.BadRequest(uploadResult.StringMessage));

            // Сопостановление имен
            var uploadedFileNames = new Queue<string>(uploadResult.Value!.Select(x => x.FileName));

            // Формируем запрос для БД
            var dbRequests = documentsToSave.Select(doc => dbRequest(doc, uploadedFileNames.TryDequeue(out var fileName) ? fileName : null)).ToList();

            // Сохраняем в БД
            var dbResult = await dbSaveAsync(dbRequests);

            if (dbResult.IsFailure)
                return Result.Failure(Error.BadRequest(dbResult.StringMessage));

            // Обновляем статусы сохранения
            foreach (var doc in documentsToSave)
                markAsSavedAction(doc);

            return Result.Success();
        }
    }
}
