using LifeLine.HrPanel.Desktop.Models;
using Terminex.Common.Results;

namespace LifeLine.HrPanel.Desktop.Services.Document.DocumentProcessing
{
    public interface IDocumentProcessingService
    {
        Task<Result<(byte[] PdfBytes, string FileName)>> ProcessFilesToPdfAsync(IEnumerable<PendingFileItem> pendingFiles, string documentTypeName, string employeeId, string name, CancellationToken cn = default);
    }
}