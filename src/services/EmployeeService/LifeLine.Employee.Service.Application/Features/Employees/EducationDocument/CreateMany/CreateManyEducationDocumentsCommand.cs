using MediatR;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.CreateMany
{
    public sealed record CreateManyEducationDocumentsCommand(Guid EmployeeId, List<CreateDataEducationDocumentCommand> EducationDocuments) : IRequest<Result>;

    public sealed record CreateDataEducationDocumentCommand(Guid EducationLevelId, Guid DocumentTypeId, string DocumentNumber, DateTime IssuedDate, string OrganizationName, string? QualificationAwardedName, string? SpecialtyName, string? ProgramName, TimeSpan? TotalHours, string? BucketName, string? FileName);
}
