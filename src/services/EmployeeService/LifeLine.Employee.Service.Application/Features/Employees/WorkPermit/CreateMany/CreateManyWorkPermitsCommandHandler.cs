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

namespace LifeLine.Employee.Service.Application.Features.Employees.WorkPermit.CreateMany
{
    public sealed class CreateManyWorkPermitsCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<CreateManyWorkPermitsCommandHandler> logger
        ) : IRequestHandler<CreateManyWorkPermitsCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateManyWorkPermitsCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateManyWorkPermitsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(Error.NotFound("Пользователь не найден!"));

                foreach (var item in request.WorkPermits)
                    employee.AddWorkPermit
                        (
                            item.WorkPermitName,
                            item.DocumentSeries,
                            item.WorkPermitNumber,
                            item.ProtocolNumber,
                            item.SpecialtyName,
                            item.IssuingAuthority,
                            item.IssueDate,
                            item.ExpiryDate,
                            item.BucketName,
                            item.FileName,
                            item.PermitTypeId,
                            item.AdmissionStatusId
                        );

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is ExistWorkPermitException existPersDocEX)
                    return Result.Failure(new Error(AppErrors.ExistWorkPermit, existPersDocEX.Message));

                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В методе '{nameof(Domain.Models.Employee.Create)}', в '{nameof(CreateManyWorkPermitsCommandHandler)}' при создании персональных документов сотрудника не был сгенерирован {nameof(WorkPermitId)}, в виде Guid!");
                    return Result.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера!"));
                }

                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при создании аккредитации / сертификатов!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
