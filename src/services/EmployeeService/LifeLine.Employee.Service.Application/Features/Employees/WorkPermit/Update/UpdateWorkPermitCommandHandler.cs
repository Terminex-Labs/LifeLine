using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Kernel.Exceptions;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.WorkPermit.Update
{
    public sealed class UpdateWorkPermitCommandHandler
        (
            IWriteContext writeContext,
            IEmployeeRepository employeeRepository,
            ILogger<UpdateWorkPermitCommandHandler> logger
        ) : IRequestHandler<UpdateWorkPermitCommand, Result>
    {
        private readonly IWriteContext _writeContext = writeContext;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<UpdateWorkPermitCommandHandler> _logger = logger;

        public async Task<Result> Handle(UpdateWorkPermitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                employee.UpdateNameWP(request.Id, request.WorkPermitName);
                employee.UpdateDocumentSeriesWP(request.Id, request.DocumentSeries);
                employee.UpdateDocumentNumberWP(request.Id, request.WorkPermitNumber);
                employee.UpdateProtocolNumberWP(request.Id, request.ProtocolNumber);
                employee.UpdateSpecialtyNameWP(request.Id, request.SpecialtyName);
                employee.UpdateIssuingAuthorityWP(request.Id, request.IssuingAuthority);
                employee.UpdateIssueDateWP(request.Id, request.IssueDate);
                employee.UpdateExpiryDateWP(request.Id, request.ExpiryDate);
                employee.UpdateFileKeyWorkPermit(request.Id, request.BucketName, request.FileName);
                employee.UpdatePermitTypeIdWP(request.Id, request.PermitTypeId);
                employee.UpdateAdmissionStatusIdWP(request.Id, request.AdmissionStatusId);

                await _writeContext.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException domainEX)
            {
                return Result.Failure(new Error(ErrorCode.Create, domainEX.Message));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при обновлении разрешения на рабооту!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
