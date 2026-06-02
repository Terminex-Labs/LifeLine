using MediatR;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Update
{
    public sealed record UpdateEducationDocumentCommand
        (
            string Id,
            string EmployeeId,
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
        ) : IRequest<Result>;
}
