using LifeLine.EmployeeService.Application.Abstraction.Common.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Response.EmployeeService;

namespace LifeLine.Employee.Service.Application.Features.Employees.Get.GetFullDetailsForEmployee
{
    public sealed class GetFullDetailsForEmployeeQueryHandler(IReadContext context) : IRequestHandler<GetFullDetailsForEmployeeQuery, EmployeeFullDetailsResponse?>
    {
        private readonly IReadContext _context = context;

        public async Task<EmployeeFullDetailsResponse?> Handle(GetFullDetailsForEmployeeQuery request, CancellationToken cancellationToken)
        {
            var employeeDetails = await _context.EmployeeFullDetailsViews.FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId, cancellationToken);

            var response = new EmployeeFullDetailsResponse
                (
                    employeeDetails.EmployeeId,
                    employeeDetails.Surname,
                    employeeDetails.Name,
                    employeeDetails.Patronymic,
                    employeeDetails.DateEntry,
                    employeeDetails.Rating,
                    employeeDetails.PersonalPhoto,

                    new GenderDetailsResponseData(employeeDetails.Gender.GenderId, employeeDetails.Gender.GenderName),

                    new ContactInformationDetailsResponseData
                        (
                            employeeDetails.ContactInformation?.ContactInformationId,
                            employeeDetails.ContactInformation?.PersonalPhone,
                            employeeDetails.ContactInformation?.CorporatePhone,
                            employeeDetails.ContactInformation?.PersonalEmail,
                            employeeDetails.ContactInformation?.CorporateEmail,
                            employeeDetails.ContactInformation?.PostalCode,
                            employeeDetails.ContactInformation?.Region,
                            employeeDetails.ContactInformation?.City,
                            employeeDetails.ContactInformation?.Street,
                            employeeDetails.ContactInformation?.Building,
                            employeeDetails.ContactInformation?.Apartment
                        ),

                    employeeDetails.Assignments?.Select
                        (
                            x => new AssignmentDetailsResponseData
                                (
                                    x.AssignmentId,
                                    x.PositionId,
                                    x.DepartmentId,
                                    x.ManagerId,
                                    x.HireDate,
                                    x.TerminationDate,
                                    x.StatusId,
                                    x.ContractId
                                )
                        ).ToList(),

                    employeeDetails.Contracts?.Select
                        (
                            x => new ContractDetailsResponseData
                                (
                                    x.ContractId,
                                    x.ContractNumber,
                                    x.EmployeeTypeId,
                                    x.ContractStartDate,
                                    x.ContractEndDate,
                                    x.Salary,
                                    x.ContractFileKey
                                )
                        ).ToList(),

                    employeeDetails.EducationDocuments?.Select
                        (
                            x => new EducationDocumentDetailsResponseData
                                (
                                    x.EducationDocumentId,
                                    x.EducationLevelId,
                                    x.EducationDocumentTypeId,
                                    x.EducationDocumentNumber,
                                    x.EducationIssuedDate,
                                    x.OrganizationName,
                                    x.QualificationAwardedName,
                                    x.EducationSpecialtyName,
                                    x.ProgramName,
                                    x.TotalHours,
                                    x.EducationDocumentFileKey
                                )
                        ).ToList(),

                    employeeDetails.PersonalDocuments?.Select
                        (
                            x => new PersonalDocumentDetailsResponseDate
                                (
                                    x.PersonalDocumentId,
                                    x.PersonalDocumentTypeId,
                                    x.PersonalDocumentNumber,
                                    x.PersonalDocumentSeries,
                                    x.PersonalDocumentFileKey
                                )
                        ).ToList(),

                    employeeDetails.Specialties?.Select
                        (
                            x => new SpecialtyDetailsResponseData
                                (
                                    x.SpecialtyId,
                                    x.SpecialtyName,
                                    x.SpecialtyDescription
                                )
                        ).ToList(),

                    employeeDetails.WorkPermits?.Select
                        (
                            x => new WorkPermitDetailsResponseData
                                (
                                    x.WorkPermitId,
                                    x.WorkPermitName,
                                    x.WorkPermitDocumentSeries,
                                    x.WorkPermitNumber,
                                    x.ProtocolNumber,
                                    x.WorkPermitSpecialtyName,
                                    x.IssuingAuthority,
                                    x.WorkPermitIssueDate,
                                    x.WorkPermitExpiryDate,
                                    x.WorkPermitFileKey,
                                    x.PermitTypeId,
                                    x.AdmissionStatusId
                                )
                        ).ToList()
                );

            return response;
        }
    }
}
