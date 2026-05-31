using Shared.Contracts.Request.Shared;

namespace Shared.Contracts.Request.EmployeeService.PersonalDocument
{
    public sealed record CreateManyPersonalDocumentsRequest(List<CreateDataPersonalDocumentRequest> PersonalDocuments);

    public sealed record CreateDataPersonalDocumentRequest(string DocumentTypeId, string DocumentNumber, string? DocumentSeries, string? BucketName, string? FileName);
}
