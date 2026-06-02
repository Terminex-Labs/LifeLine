using LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Create;
using LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.CreateMany;
using LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Delete;
using LifeLine.Employee.Service.Application.Features.Employees.EducationDocument.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Request.EmployeeService.EducationDocument;

namespace LifeLine.Employee.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/employees/{employeeId}/education-documents")]
    public class EducationDocumentController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] Guid employeeId, [FromBody] CreateEducationDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreateEducationDocumentCommand
                (
                    employeeId,
                    request.EducationLevelId,
                    request.DocumentTypeId,
                    request.DocumentNumber,
                    request.IssuedDate,
                    request.OrganizationName,
                    request.QualificationAwardedName,
                    request.SpecialtyName,
                    request.ProgramName,
                    request.TotalHours != null ? TimeSpan.FromHours(request.TotalHours.Value) : TimeSpan.Zero
                );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное создание!"),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpPost("many")]
        public async Task<IActionResult> CreateMany([FromRoute] Guid employeeId, [FromBody] CreateManyEducationDocumentsReqeust reqeust, CancellationToken cancellationToken = default)
        {
            var command = new CreateManyEducationDocumentsCommand
                (
                    employeeId,
                    [.. reqeust.EducationDocuments.Select
                        (
                            x => new CreateDataEducationDocumentCommand
                                (
                                    Guid.Parse(x.EducationLevelId),
                                    Guid.Parse(x.DocumentTypeId),
                                    x.DocumentNumber,
                                    DateTime.Parse(x.IssuedDate),
                                    x.OrganizationName,
                                    x.QualificationAwardedName,
                                    x.SpecialtyName,
                                    x.ProgramName,
                                    x.TotalHours,
                                    x.BucketName,
                                    x.FileName
                                )
                        )
                    ]
                );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok(),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpPatch("{educationDocumentId}")]
        public async Task<IActionResult> Update([FromRoute] Guid employeeId, [FromRoute] Guid educationDocumentId, [FromBody] UpdateEducationDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateEducationDocumentCommand
                (
                    educationDocumentId.ToString(), 
                    employeeId.ToString(), 
                    request.EducationLevelId, 
                    request.DocumentTypeId, 
                    request.DocumentNumber, 
                    request.IssuedDate, 
                    request.OrganizationName, 
                    request.QualificationAwardedName, 
                    request.SpecialtyName, 
                    request.ProgramName, 
                    request.TotalHours, 
                    request.BucketName, 
                    request.FileName
                );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное обновление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpDelete("{educationDocumentId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid employeeId, [FromRoute] Guid educationDocumentId, CancellationToken cancellationToken = default)
        {
            var command = new DeleteEducationDocumentCommand(educationDocumentId, employeeId);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное удаление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }
    }
}
