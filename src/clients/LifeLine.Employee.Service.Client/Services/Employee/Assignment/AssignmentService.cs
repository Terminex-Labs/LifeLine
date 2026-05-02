using Shared.Contracts.Request.EmployeeService.Assignment;
using Shared.Contracts.Response.EmployeeService;
using Shared.Http.Base;
using Shared.Kernel.Errors;
using System.Diagnostics;
using System.Net.Http.Json;
using Terminex.Common.Results;

namespace LifeLine.Employee.Service.Client.Services.Employee.Assignment
{
    public sealed class AssignmentService(HttpClient httpClient, string employeeId) :
        BaseHttpService<AssignmentResponse, string>(httpClient, $"api/employees/{employeeId}/assignments"), IAssignmentService
    {
        public async Task<Result> CreateManyAsync(CreateManyAssignmentsReqeust reqeust)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{Url}/many", reqeust, JsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(AppErrors.CreateHttp, $"Произошла ошибка при сохранении данных в назначении!\n{ex}"));
            }
        }

        public async Task<Result> UpdateAssignmentAsync(Guid assignmentId, Guid contractId, UpdateAssignmentRequest request)
        {
            try
            {
                var response = await HttpClient.PatchAsJsonAsync($"{Url}/{assignmentId}/{contractId}", request, JsonSerializerOptions);
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(AppErrors.UpdateHttp, $"Произошла ошибка при изменении данных в назначении!\n{ex}"));
            }
        }

        public async Task<Result> DeleteAssignmentContractAsync(Guid assignmentId)
        {
            try
            {
                var response = await HttpClient.DeleteAsync($"{Url}/{assignmentId}");
                response.EnsureSuccessStatusCode();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new Error(AppErrors.DeleteHttp, $"Произошла ошибка при удалении назначения!\n{ex}"));
            }
        }
    }
}
