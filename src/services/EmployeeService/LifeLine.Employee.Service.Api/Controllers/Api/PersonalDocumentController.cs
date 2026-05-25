using LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.Create;
using LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.CreateMany;
using LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.Delete;
using LifeLine.Employee.Service.Application.Features.Employees.PersonalDocuments.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Request.EmployeeService.PersonalDocument;

namespace LifeLine.Employee.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/employees/{employeeId}/personal-documents")]
    public class PersonalDocumentController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] Guid employeeId, [FromBody] CreatePersonalDocumentRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreatePersonalDocumentCommand(employeeId, request.DocumentTypeId, request.DocumentNumber, request.DocumentSeries, null);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok(),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpPost("many")]
        public async Task<IActionResult> CreateMany([FromRoute] Guid employeeId, [FromBody] CreateManyPersonalDocumentsRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreateManyPersonalDocumentsCommand
                (
                    employeeId,
                    [.. request.PersonalDocuments.Select
                        (
                            x => new CreateDataPersonalDocumentCommand
                                (
                                    Guid.Parse(x.DocumentTypeId),
                                    x.DocumentNumber,
                                    x.DocumentSeries,
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

        [HttpPatch("{personalDocumentId}")]
        public async Task<IActionResult> Update([FromRoute] Guid employeeId, [FromRoute] Guid personalDocumentId, [FromBody] UpdatePersonalDocumentRequest request, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Пришло");
            var command = new UpdatePersonalDocumentCommand(personalDocumentId.ToString(), employeeId.ToString(), request.DocumentTypeId, request.DocumentNumber, request.DocumentSeries);

            var result = await _mediator.Send(command, cancellationToken);


            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное обновление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpDelete("{personalDocumentId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid employeeId, [FromRoute] Guid personalDocumentId, CancellationToken cancellationToken = default)
        {
            var command = new DeletePersonalDocuemtnCommand(personalDocumentId, employeeId);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное удаление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }
    }
}
