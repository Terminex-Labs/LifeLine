namespace Shared.Contracts.Response.EmployeeService
{
    public sealed record WorkPermitResponse(string Id, string EmployeeId, string WorkPermitName, string? DocumentSeries, string WorkPermitNumber, string? ProtocolNumber, string SpecialtyName, string IssuingAuthority, DateTime IssueDate, DateTime ExpiryDate, string? FileKey, string PermitTypeId, string AdmissionStatusId);
}
