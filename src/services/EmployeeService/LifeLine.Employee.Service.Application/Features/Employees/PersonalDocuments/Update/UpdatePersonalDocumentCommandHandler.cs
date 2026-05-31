using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.Update
{
    public sealed class UpdatePersonalDocumentCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<UpdatePersonalDocumentCommandHandler> logger
        ) : IRequestHandler<UpdatePersonalDocumentCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<UpdatePersonalDocumentCommandHandler> _logger = logger;

        public async Task<Result> Handle(UpdatePersonalDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(Guid.Parse(request.EmployeeId));

                if (employee is null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.UpdateDocumentTypePD(Guid.Parse(request.Id), Guid.Parse(request.DocumentTypeId));
                employee.UpdateDocumentNumberPD(Guid.Parse(request.Id), request.DocumentNumber);
                employee.UpdateDocumentSeries(Guid.Parse(request.Id), request.DocumentSeries);
                employee.UpdateFileKeyPersonalDocument(Guid.Parse(request.Id), request.BucketName, request.FileName);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при обновлении персонального документа!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
