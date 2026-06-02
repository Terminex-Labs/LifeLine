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

namespace LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.CreateMany
{
    public sealed class CreateManyEducationDocumentsCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<CreateManyEducationDocumentsCommandHandler> logger
        ) : IRequestHandler<CreateManyEducationDocumentsCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<CreateManyEducationDocumentsCommandHandler> _logger = logger;

        public async Task<Result> Handle(CreateManyEducationDocumentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(Error.NotFound("Пользователь не найден!"));

                foreach (var item in request.EducationDocuments)
                    employee.AddEducationDocument
                        (
                            item.EducationLevelId,
                            item.DocumentTypeId,
                            item.DocumentNumber,
                            item.IssuedDate,
                            item.OrganizationName,
                            item.QualificationAwardedName,
                            item.SpecialtyName,
                            item.ProgramName,
                            item.TotalHours,
                            item.BucketName,
                            item.FileName
                        );

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                if (domainEX is ExistEducationDocumentException existEducDocEX)
                    return Result.Failure(new Error(AppErrors.ExistEducationDocument, existEducDocEX.Message));

                if (domainEX is EmptyIdentifierException emptyEX)
                {
                    _logger.LogCritical(emptyEX, $"В методе '{nameof(Domain.Models.Employee.Create)}', в '{nameof(CreateManyEducationDocumentsCommand)}' при создании персональных документов сотрудника не был сгенерирован {nameof(EducationDocumentId)}, в виде Guid!");
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
