using LifeLine.Employee.Service.Domain.Exceptions;
using LifeLine.Employee.Service.Domain.ValueObjects.WorkPermits;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;
using Shared.Kernel.Errors;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.WorkPermit.Create
{
    public sealed class CreateWorkPermitCommandHandler
        (
            IWriteContext context,
            IEmployeeRepository employeeRepository,
            ILogger<CreateWorkPermitCommandHandler> logger
        ) : IRequestHandler<CreateWorkPermitCommand, Result>
    {
        private readonly IWriteContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateWorkPermitCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateWorkPermitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.AddWorkPermit
                    (
                        request.WorkPermitName,
                        request.DocumentSeries,
                        request.WorkPermitNumber,
                        request.ProtocolNumber,
                        request.SpecialtyName,
                        request.IssuingAuthority,
                        request.IssueDate,
                        request.ExpiryDate,
                        null,
                        null,
                        request.PermitTypeId,
                        request.AdmissionStatusId
                    );

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is ExistContactInformationException existContInfoEX)
                    return Result.Failure(new Error(AppErrors.ExistEducationInformation, existContInfoEX.Message));

                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В методе '{nameof(Domain.Models.WorkPermit.Create)}', в '{nameof(CreateWorkPermitCommandHandler)}' при создании сертификатов сотрудника не был сгенерирован {nameof(WorkPermitId)}, в виде Guid!");
                    return Result.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера!"));
                }

                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при создании сертификатов!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
