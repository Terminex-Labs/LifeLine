namespace Shared.Contracts.Request.EmployeeService.Assignment
{
    public sealed record CreateManyAssignmentsReqeust(List<CreateManyDataAssignmentsReqeust> Assignments);

    public sealed record CreateManyDataAssignmentsReqeust(string PositionId, string DepartmentId, string? ManagerId, DateTime HireDate, DateTime? TerminationDate, string StatusId, CreateManyDataAssignmentContractReqeust Contracts);

    public sealed record CreateManyDataAssignmentContractReqeust(string EmployeeTypeId, string ContractNumber, DateTime StartDate, DateTime EndDate, decimal Salary, string? BucketName, string? FileName);
}
