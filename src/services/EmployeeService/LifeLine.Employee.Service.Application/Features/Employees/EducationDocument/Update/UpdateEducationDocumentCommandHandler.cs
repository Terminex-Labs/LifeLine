using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Update
{
    public sealed class UpdateEducationDocumentCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<UpdateEducationDocumentCommandHandler> logger
        ) : IRequestHandler<UpdateEducationDocumentCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<UpdateEducationDocumentCommandHandler> _logger = logger;

        public async Task<Result> Handle(UpdateEducationDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(Guid.Parse(request.EmployeeId));

                if (employee is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.UpdateEducationLevel(Guid.Parse(request.Id), Guid.Parse(request.EducationLevelId));
                employee.UpdateDocumentTypeED(Guid.Parse(request.Id), Guid.Parse(request.DocumentTypeId));
                employee.UpdateDocumentNumberED(Guid.Parse(request.Id), request.DocumentNumber);
                employee.UpdateIssuedDate(Guid.Parse(request.Id), request.IssuedDate);
                employee.UpdateOrganizationName(Guid.Parse(request.Id), request.OrganizationName);
                employee.UpdateQualificationAwardedName(Guid.Parse(request.Id), request.QualificationAwardedName);
                employee.UpdateSpecialtyName(Guid.Parse(request.Id), request.SpecialtyName);
                employee.UpdateProgramName(Guid.Parse(request.Id), request.ProgramName);
                employee.UpdateTotalHours(Guid.Parse(request.Id), request.TotalHours);
                employee.UpdateFileKeyEducationDocument(Guid.Parse(request.Id), request.BucketName, request.FileName);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при обновлении послеучебного документа!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
