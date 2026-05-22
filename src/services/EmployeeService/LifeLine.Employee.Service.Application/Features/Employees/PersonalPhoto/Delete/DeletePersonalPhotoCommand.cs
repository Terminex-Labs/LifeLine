using MediatR;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.PersonalPhoto.Delete
{
    public sealed record DeletePersonalPhotoCommand(Guid EmployeeId) : IRequest<Result>;
}
