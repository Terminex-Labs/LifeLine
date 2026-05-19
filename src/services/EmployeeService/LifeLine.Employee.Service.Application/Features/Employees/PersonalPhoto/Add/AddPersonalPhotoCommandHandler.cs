using LifeLine.Employee.Service.Domain.ValueObjects.Employees;
using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using LifeLine.EmployeeService.Application.Abstraction.Common.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Application.Features.Employees.PersonalPhoto.Add
{
    public sealed class AddPersonalPhotoCommandHandler
        (
            IWriteContext context,
            IEmployeeRepository employeeRepository,
            ILogger<AddPersonalPhotoCommandHandler> logger
        ) : IRequestHandler<AddPersonalPhotoCommand, Result>
    {
        private readonly IWriteContext _context = context;
        private readonly IEmployeeRepository _employeeRepository = employeeRepository;
        private readonly ILogger<AddPersonalPhotoCommandHandler> _logger = logger;

        public async Task<Result> Handle(AddPersonalPhotoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

                if (employee == null)
                    return Result.Failure(new Error(ErrorCode.NotFound, "Пользователь не найден!"));

                var fileUrlResult = FileUrl.Create(request.BucketName, request.FileName);

                if (fileUrlResult.IsFailure)
                    return fileUrlResult;

                employee.AddPersonalPhoto(fileUrlResult.Value);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка при добавлении аватара сотрудника!");

                return Result.Failure(new Error(ErrorCode.Server, "Ошибка сервера при сохранении!"));
            }
        }
    }
}
