namespace Shared.Contracts.Response.EmployeeService
{
    public sealed record PersonalDocumentResponse(Guid Id, Guid DocumentTypeId, string Number, string? Series, string? FileKey);
}
