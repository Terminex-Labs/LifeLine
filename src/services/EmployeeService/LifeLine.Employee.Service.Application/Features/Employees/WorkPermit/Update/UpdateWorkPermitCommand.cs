using MediatR;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.WorkPermit.Update
{
    public sealed record UpdateWorkPermitCommand
        (
            Guid Id,
            Guid EmployeeId,
            string WorkPermitName, 
            string? DocumentSeries, 
            string WorkPermitNumber, 
            string? ProtocolNumber, 
            string SpecialtyName, 
            string IssuingAuthority, 
            DateTime IssueDate, 
            DateTime ExpiryDate,
            string? BucketName,
            string? FileName,
            Guid PermitTypeId, 
            Guid AdmissionStatusId
        ) : IRequest<Result>;
}
