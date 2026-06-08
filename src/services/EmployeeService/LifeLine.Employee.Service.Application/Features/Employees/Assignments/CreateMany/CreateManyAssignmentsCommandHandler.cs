using LifeLine.Employee.Service.Domain.Models;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.Assignments.CreateMany
{
    public sealed class CreateManyAssignmentsCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<CreateManyAssignmentsCommandHandler> logger
        ) : IRequestHandler<CreateManyAssignmentsCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateManyAssignmentsCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateManyAssignmentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(Error.NotFound("Пользователь не найден!"));

                foreach (var assignmentData in request.Assignments)
                {
                    var contract = employee.AddContract
                        (
                            assignmentData.Contracts.EmployeeTypeId,
                            assignmentData.Contracts.ContractNumber,
                            assignmentData.Contracts.StartDate,
                            assignmentData.Contracts.EndDate,
                            assignmentData.Contracts.Salary,
                            assignmentData.Contracts.BucketName,
                            assignmentData.Contracts.FileName
                        );

                    employee.AddAssignment
                        (
                            assignmentData.PositionId,
                            assignmentData.DepartmentId,
                            assignmentData.ManagerId,
                            assignmentData.HireDate,
                            assignmentData.TerminationDate,
                            assignmentData.StatusId,
                            contract.Id 
                        );
                }

                await _writeContext.SaveChangesAsync();

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В процессе вызова '{nameof(CreateManyAssignmentsCommandHandler)}' при создании занятости {nameof(Assignment)} и/или контракт {nameof(Contract)}, не был сгенерирован Guid!");
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
