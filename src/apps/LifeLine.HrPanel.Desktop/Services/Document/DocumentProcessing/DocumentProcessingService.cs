using Terminex.Common.Results;
using Shared.WPF.Services.Conversion;
using LifeLine.HrPanel.Desktop.Models;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentProcessing
{
    public sealed class DocumentProcessingService(IDocumentConversionService conversionService) : IDocumentProcessingService
    {
        public async Task<Result<(byte[] PdfBytes, string FileName)>> ProcessFilesToPdfAsync
            (
                IEnumerable<PendingFileItem> pendingFiles,
                string documentTypeName,
                string employeeId,
                string number,
                CancellationToken cn = default
            )
        {
            var validFiles = pendingFiles.Where(x => System.IO.File.Exists(x.FilePath)).ToList();

            if (validFiles.Count <= 0)
                return Result<(byte[], string)>.Failure(Error.Validation("Не удается прочитать выбранные файлы!"));

            var fileBytes = new List<byte[]>();
            var fileNames = new List<string>();

            foreach (var file in validFiles)
            {
                fileBytes.Add(await System.IO.File.ReadAllBytesAsync(file.FilePath!, cn));
                fileNames.Add(System.IO.Path.GetFileName(file.FilePath!));
            }

            var pdfBytes = await conversionService.ConvertImagesToPdfAsync
                (
                    documentTypeName,
                    employeeId,
                    fileBytes,
                    fileNames,
                    cn
                );

            var fileName = $"{number}.pdf";

            return Result<(byte[], string)>.Success((pdfBytes, fileName));
        }
    }
}
