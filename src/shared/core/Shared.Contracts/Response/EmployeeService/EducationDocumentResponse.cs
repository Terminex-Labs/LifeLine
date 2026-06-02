namespace Shared.Contracts.Response.EmployeeService
{
    public sealed record EducationDocumentResponse(string Id, string EmployeeId, string EducationLevelId, string DocumentTypeId, string DocumentNumber, string IssuedDate, string OrganizationName, string? QualificationAwardedName, string? SpecialtyName, string? ProgramName, string? TotalHours, string? FileKey);
}
