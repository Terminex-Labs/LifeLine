using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Response.EmployeeService;

namespace LifeLine.Employee.Service.Application.Features.Employees.Get.GetAllForHr
{
    public sealed class GetAllEmployeeForHrQueryHandler(IReadContext context) : IRequestHandler<GetAllEmployeeForHrQuery, List<EmployeeHrItemResponse>>
    {
        private readonly IReadContext _context = context;

        public async Task<List<EmployeeHrItemResponse>> Handle(GetAllEmployeeForHrQuery request, CancellationToken cancellationToken)
            => await _context.EmployeeHrItemViews
                //.Where(x => x.IsActive == true)
                .Select
                (
                    x => new EmployeeHrItemResponse
                        (
                            x.Id.ToString(), 
                            x.Surname, 
                            x.Name, 
                            x.Patronymic,
                            x.PersonalPhoto,
                            x.IsActive,
                            x.Assignments.Select
                                (
                                    x => new AssignmentResponseInfo
                                        (
                                            x.DepartmentId.ToString(), 
                                            x.PositionId.ToString(), 
                                            x.StatusId.ToString()
                                        )
                                ).ToList()
                        )
                ).ToListAsync(cancellationToken);
    }
}
