using LifeLine.Employee.Service.Domain.Exceptions;
using LifeLine.Employee.Service.Domain.ValueObjects.EducationDocuments;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Domain.Exceptions;
using Shared.Kernel.Errors;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Create
{
    public sealed class CreateEducationDocumentCommandHandler
        (
            IWriteContext context,
            IEmployeeRepository employeeRepository,
            ILogger<CreateEducationDocumentCommandHandler> logger
        ) : IRequestHandler<CreateEducationDocumentCommand, Result>
    {
        private readonly IWriteContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateEducationDocumentCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateEducationDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.AddEducationDocument
                    (
                        request.EducationLevelId,
                        request.DocumentTypeId,
                        request.DocumentNumber,
                        request.IssuedDate,
                        request.OrganizationName,
                        request.QualificationAwardedName,
                        request.SpecialtyName,
                        request.ProgramName,
                        request.TotalHours,
                        null, 
                        null
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
                    _logger.LogCritical(emptyEX, $"В методе '{nameof(Domain.Models.EducationDocument.Create)}', в '{nameof(CreateEducationDocumentCommandHandler)}' при создании документов об образовании сотрудника не был сгенерирован {nameof(EducationDocumentId)}, в виде Guid!");
                    return Result.Failure(new Error(ErrorCode.Create, "Ошибка на стороне сервера!"));
                }

                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при создании документов об образовании!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
