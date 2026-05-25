using LifeLine.Employee.Service.Domain.Exceptions;
using LifeLine.Employee.Service.Domain.ValueObjects.PersonalDocuments;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;
using Shared.Kernel.Errors;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.Create
{
    public sealed class CreatePersonalDocumentCommandHandler
        (
            IWriteContext context,
            IEmployeeRepository employeeRepository,
            ILogger<CreatePersonalDocumentCommandHandler> logger
        ) : IRequestHandler<CreatePersonalDocumentCommand, Result>
    {
        private readonly IWriteContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreatePersonalDocumentCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreatePersonalDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.AddPersonalDocument(request.DocumentTypeId, request.DocumentNumber, request.DocumentSeries, null, null);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is ExistPersonalDocumentException existPersDocEX)
                    return Result.Failure(new Error(AppErrors.ExistPersonalDocument, existPersDocEX.Message));

                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В методе '{nameof(Domain.Models.Employee.Create)}', в '{nameof(CreatePersonalDocumentCommandHandler)}' при создании персональных документов сотрудника не был сгенерирован {nameof(PersonalDocumentId)}, в виде Guid!");
                    return Result.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера!"));
                }

                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при создании личных документов!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
