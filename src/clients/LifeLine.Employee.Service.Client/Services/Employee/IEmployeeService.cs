using Shared.Contracts.Request.EmployeeService.Employee;
using Shared.Contracts.Response.EmployeeService;
using Shared.Http.Base;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Client.Services.Employee
{
    public interface IEmployeeService : IBaseHttpService<EmployeeResponse, string>
    {
        Task<Result> AddPersonalPhoto(string employeeId, AddPersonalPhotoRequest request);
        Task<Result> DeletePersonalPhoto(string employeeId);
        Task<List<EmployeeHrItemResponse>> GetAllForHrAsync();
        Task<EmployeeFullDetailsResponse?> GetDetailsAsync(string id);
        Task<Result> UpdateEmployeeAsync(string employeeId, UpdateEmployeeRequest request);
        Task<Result> SoftDeleteAsync(string employeeId);
    }
}
