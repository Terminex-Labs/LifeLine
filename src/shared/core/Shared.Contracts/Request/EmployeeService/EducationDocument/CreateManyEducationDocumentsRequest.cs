namespace Shared.Contracts.Request.EmployeeService.EducationDocument
{
    public sealed record CreateManyEducationDocumentsReqeust(List<CreateDataEducationDocumentReqeust> EducationDocuments);

    public sealed record CreateDataEducationDocumentReqeust(string EducationLevelId, string DocumentTypeId, string DocumentNumber, string IssuedDate, string OrganizationName, string? QualificationAwardedName, string? SpecialtyName, string? ProgramName, TimeSpan? TotalHours, string? BucketName, string? FileName);
}
