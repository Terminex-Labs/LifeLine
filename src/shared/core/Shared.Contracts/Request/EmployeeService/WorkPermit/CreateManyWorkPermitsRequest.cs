namespace Shared.Contracts.Request.EmployeeService.WorkPermit
{
    public sealed record CreateManyWorkPermitsRequest(List<CreateManyDataWorkPermitsRequest> WorkPermits);

    public sealed record CreateManyDataWorkPermitsRequest(string WorkPermitName, string? DocumentSeries, string WorkPermitNumber, string? ProtocolNumber, string SpecialtyName, string IssuingAuthority, string IssueDate, string ExpiryDate, string? BucketName, string? FileName, string PermitTypeId, string AdmissionStatusId);
}
