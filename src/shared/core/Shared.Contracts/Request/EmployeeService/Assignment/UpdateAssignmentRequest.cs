namespace Shared.Contracts.Request.EmployeeService.Assignment
{
    public sealed record UpdateAssignmentRequest
        (
            Guid PositionId,
            Guid DepartmentId,
            Guid? ManagerId,
            DateTime HireDate,
            DateTime? TerminationDate,
            Guid StatusId,
            UpdateAssignmentContractRequest Contract
        );

    public sealed record UpdateAssignmentContractRequest
        (
            Guid EmployeeTypeId,
            string ContractNumber,
            DateTime StartDate,
            DateTime EndDate,
            decimal Salary,
            string? BucketName,
            string? FileName
        );
}
