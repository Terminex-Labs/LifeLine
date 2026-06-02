namespace Shared.Contracts.Request.EmployeeService.EducationDocument
{
    public sealed record UpdateEducationDocumentRequest
        (
            string EducationLevelId, 
            string DocumentTypeId, 
            string DocumentNumber, 
            DateTime IssuedDate, 
            string OrganizationName, 
            string? QualificationAwardedName, 
            string? SpecialtyName, 
            string? ProgramName, 
            TimeSpan? TotalHours,
            string? BucketName,
            string? FileName
        );
}
