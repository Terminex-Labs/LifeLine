namespace LifeLine.EmployeeService.Application.Abstraction.Models
{
    public record EmployeeFullDetailsView
        (
            Guid EmployeeId,
            string Surname,
            string Name,
            string? Patronymic,
            DateTime DateEntry,
            double Rating,
            string? PersonalPhoto,

            GenderDetailsViewData Gender,
            ContactInformationDetailsViewData? ContactInformation,
            List<AssignmentDetailsViewData>? Assignments,
            List<ContractDetailsViewData>? Contracts,
            List<EducationDocumentDetailsViewData>? EducationDocuments,
            List<PersonalDocumentDetailsViewDate>? PersonalDocuments,
            List<SpecialtyDetailsViewData>? Specialties,
            List<WorkPermitDetailsViewData>? WorkPermits
        );

    public record GenderDetailsViewData
        (
            Guid GenderId,
            string GenderName
        );

    public record ContactInformationDetailsViewData
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

    public record AssignmentDetailsViewData
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

    public record ContractDetailsViewData
        (
            Guid ContractId,
            string ContractNumber,
            Guid EmployeeTypeId,
            DateTime ContractStartDate,
            DateTime ContractEndDate,
            decimal Salary,
            string? ContractFileKey
        );

    public record EducationDocumentDetailsViewData
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

    public record PersonalDocumentDetailsViewDate
        (
            Guid PersonalDocumentId,
            Guid PersonalDocumentTypeId,
            string PersonalDocumentNumber,
            string? PersonalDocumentSeries,
            string? PersonalDocumentFileKey
        );

    public record SpecialtyDetailsViewData
        (
            Guid SpecialtyId,
            string SpecialtyName,
            string? SpecialtyDescription
        );

    public record WorkPermitDetailsViewData
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
