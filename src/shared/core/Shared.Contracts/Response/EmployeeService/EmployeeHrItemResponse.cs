namespace Shared.Contracts.Response.EmployeeService
{
    public sealed record EmployeeHrItemResponse(string Id, string Surname, string Name, string? Patronymic, string? PersonalPhoto, bool IsActive, List<AssignmentResponseInfo> Assignments);

    public sealed record AssignmentResponseInfo(string DepartmentId, string PositionId, string StatusId);
}
