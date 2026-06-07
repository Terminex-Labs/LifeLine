using MediatR;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.WorkPermit.CreateMany
{
    public sealed record CreateManyWorkPermitsCommand(Guid EmployeeId, List<CreateManyDataWorkPermitsCommand> WorkPermits) : IRequest<Result>;

    public sealed record CreateManyDataWorkPermitsCommand(string WorkPermitName, string? DocumentSeries, string WorkPermitNumber, string? ProtocolNumber, string SpecialtyName, string IssuingAuthority, DateTime IssueDate, DateTime ExpiryDate, string? BucketName, string? FileName, Guid PermitTypeId, Guid AdmissionStatusId);
}
