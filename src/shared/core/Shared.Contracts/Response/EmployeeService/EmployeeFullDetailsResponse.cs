namespace Shared.Contracts.Response.EmployeeService
{
    public sealed record EmployeeFullDetailsResponse
        (
            Guid EmployeeId,
            string Surname,
            string Name,
            string? Patronymic,
            DateTime DateEntry,
            double Rating,
            string? PersonalPhoto,

            GenderDetailsResponseData Gender,
            ContactInformationDetailsResponseData? ContactInformation,
            List<AssignmentDetailsResponseData>? Assignments,
            List<ContractDetailsResponseData>? Contracts,
            List<EducationDocumentDetailsResponseData>? EducationDocuments,
            List<PersonalDocumentDetailsResponseDate>? PersonalDocuments,
            List<SpecialtyDetailsResponseData>? Specialties,
            List<WorkPermitDetailsResponseData>? WorkPermits
        );

    public record GenderDetailsResponseData
        (
            Guid GenderId,
            string GenderName
        );

    public record ContactInformationDetailsResponseData
        (
            string? ContactInformationId,
            string? PersonalPhone,
            string? CorporatePhone,
            string? PersonalEmail,
            string? CorporateEmail,
            string? PostalCode,
            string? Region,
            string? City,
            string? Street,
            string? Building,
            string? Apartment
        );

    public record AssignmentDetailsResponseData
        (
            Guid AssignmentId,
            Guid PositionId,
            Guid DepartmentId,
            Guid? ManagerId,
            DateTime HireDate,
            DateTime? TerminationDate,
            Guid StatusId,
            Guid ContractId
        );

    public record ContractDetailsResponseData
        (
            Guid ContractId,
            string ContractNumber,
            Guid EmployeeTypeId,
            DateTime ContractStartDate,
            DateTime ContractEndDate,
            decimal Salary,
            string? ContractFileKey
        );

    public record EducationDocumentDetailsResponseData
        (
            Guid EducationDocumentId,
            Guid EducationLevelId,
            Guid EducationDocumentTypeId,
            string EducationDocumentNumber,
            DateTime EducationIssuedDate,
            string OrganizationName,
            string? QualificationAwardedName,
            string? EducationSpecialtyName,
            string? ProgramName,
            double? TotalHours,
            string? EducationDocumentFileKey
        );

    public record PersonalDocumentDetailsResponseDate
        (
            Guid PersonalDocumentId,
            Guid PersonalDocumentTypeId,
            string PersonalDocumentNumber,
            string? PersonalDocumentSeries,
            string? PersonalDocumentFileKey
        );

    public record SpecialtyDetailsResponseData
        (
            Guid SpecialtyId,
            string SpecialtyName,
            string? SpecialtyDescription
        );

    public record WorkPermitDetailsResponseData
        (
            Guid WorkPermitId,
            string WorkPermitName,
            string? WorkPermitDocumentSeries,
            string WorkPermitNumber,
            string? ProtocolNumber,
            string WorkPermitSpecialtyName,
            string IssuingAuthority,
            DateTime WorkPermitIssueDate,
            DateTime WorkPermitExpiryDate,
            string? WorkPermitFileKey,
            Guid PermitTypeId,
            Guid AdmissionStatusId
        );
}
