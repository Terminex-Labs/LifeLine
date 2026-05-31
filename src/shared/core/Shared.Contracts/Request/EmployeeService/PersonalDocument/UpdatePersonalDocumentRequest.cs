namespace Shared.Contracts.Request.EmployeeService.PersonalDocument
{
    public sealed record UpdatePersonalDocumentRequest
        (
            string DocumentTypeId,
            string DocumentNumber,
            string? DocumentSeries,
            string? BucketName,
            string? FileName
        );
}
