using LifeLine.Employee.Service.Domain.Models;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.Assignments.Create
{
    public sealed class CreateAssignmentCommandHandler
        (
            IWriteContext context,
            IEmployeeRepository employeeRepository,
            ILogger<CreateAssignmentCommandHandler> logger
        ) : IRequestHandler<CreateAssignmentCommand, Result>
    {
        private readonly IWriteContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateAssignmentCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                var contract = employee.AddContract
                    (
                        request.Contract.EmployeeTypeId,
                        request.Contract.ContractNumber,
                        request.Contract.StartDate,
                        request.Contract.EndDate,
                        request.Contract.Salary,
                        null,
                        null
                    );

                employee.AddAssignment
                    (
                        request.PositionId,
                        request.DepartmentId,
                        request.ManagerId,
                        request.HireDate,
                        request.TerminationDate,
                        request.StatusId,
                        contract.Id
                    );
                
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В процессе вызова '{nameof(CreateAssignmentCommandHandler)}' при создании занятости {nameof(Assignment)} и/или контракт {nameof(Contract)}, не был сгенерирован Guid!");
                    return Result.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера!"));
                }

                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при создании занятости или контракта!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
