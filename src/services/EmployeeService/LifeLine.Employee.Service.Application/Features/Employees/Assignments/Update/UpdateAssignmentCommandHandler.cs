using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.Assignments.Update
{
    public sealed class UpdateAssignmentCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<UpdateAssignmentCommandHandler> logger
        ) : IRequestHandler<UpdateAssignmentCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<UpdateAssignmentCommandHandler> _logger = logger;

        public async Task<Result> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.UpdateAssignmentPositionId(request.Id, request.PositionId);
                employee.UpdateAssignmentDepartmentId(request.Id, request.DepartmentId);
                employee.UpdateAssignmentManagerId(request.Id, request.ManagerId);
                employee.UpdateAssignmentHireDate(request.Id, request.HireDate);
                employee.UpdateAssignmentTerminationDate(request.Id, request.TerminationDate);
                employee.UpdateAssignmentStatusId(request.Id, request.StatusId);

                employee.UpdateAssignmentContractEmploymentTypeId(request.Id, request.Contract.EmployeeTypeId);
                employee.UpdateAssignmentContractContractNumber(request.Id, request.Contract.ContractNumber);
                employee.UpdateAssignmentContractStartDate(request.Id, request.Contract.StartDate);
                employee.UpdateAssignmentContractEndDate(request.Id, request.Contract.EndDate);
                employee.UpdateAssignmentContractSalary(request.Id, request.Contract.Salary);
                employee.UpdateAssignmentContractFileKey(request.Id, request.Contract.BucketName, request.Contract.FileName);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при обновлении назначения и контракта!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
