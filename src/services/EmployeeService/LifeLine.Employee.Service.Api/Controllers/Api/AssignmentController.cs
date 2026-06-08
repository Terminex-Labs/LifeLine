using LifeLine.Employee.Service.Application.Features.Employees.Assignments.Create;
using LifeLine.Employee.Service.Application.Features.Employees.Assignments.CreateMany;
using LifeLine.Employee.Service.Application.Features.Employees.Assignments.Delete;
using LifeLine.Employee.Service.Application.Features.Employees.Assignments.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Request.EmployeeService.Assignment;

namespace LifeLine.Employee.Service.Api.Controllers.Api
{
    [ApiController]
    [Route("api/employees/{employeeId}/assignments")]
    public class AssignmentController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromRoute] Guid employeeId, [FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreateAssignmentCommand
                (
                    employeeId, 
                    request.PositionId, 
                    request.DepartmentId, 
                    request.ManagerId, 
                    request.HireDate, 
                    request.TerminationDate, 
                    request.StatusId, 
                    new CreateAssignmentContractCommand
                    (
                        request.Contract.EmployeeTypeId, 
                        request.Contract.ContractNumber, 
                        request.Contract.StartDate, 
                        request.Contract.EndDate, 
                        request.Contract.Salary, 
                        null
                    )
                );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное создание!"),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpPost("many")]
        public async Task<IActionResult> CreateMany([FromRoute] Guid employeeId, [FromBody] CreateManyAssignmentsReqeust reqeust, CancellationToken cancellationToken = default)
        {
            var command = new CreateManyAssignmentsCommand
                (
                    employeeId,
                    [.. reqeust.Assignments.Select
                        (
                            x => new CreateManyDataAssignmentsCommand
                                (
                                    Guid.Parse(x.PositionId),
                                    Guid.Parse(x.DepartmentId),
                                    !string.IsNullOrWhiteSpace(x.ManagerId) ? Guid.Parse(x.ManagerId) : null,
                                    x.HireDate,
                                    x.TerminationDate,
                                    Guid.Parse(x.StatusId),
                                    new CreateManyDataAssignmentContractCommand
                                        (
                                            Guid.Parse(x.Contracts.EmployeeTypeId),
                                            x.Contracts.ContractNumber,
                                            x.Contracts.StartDate,
                                            x.Contracts.EndDate,
                                            x.Contracts.Salary,
                                            x.Contracts.BucketName,
                                            x.Contracts.FileName
                                        )
                                )
                        )                    
                    ]
                );

            var result = await _mediator.Send(command);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok(),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpPatch("{assignmentId}/{contractId}")]
        public async Task<IActionResult> Update([FromRoute] Guid employeeId, [FromRoute] Guid assignmentId, [FromRoute] Guid contractId, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
        {
            var command = new UpdateAssignmentCommand
                (
                    assignmentId,
                    employeeId,
                    request.PositionId,
                    request.DepartmentId,
                    request.ManagerId,
                    request.HireDate,
                    request.TerminationDate,
                    request.StatusId,
                    new UpdateAssignmentContractCommand
                    (
                        contractId,
                        employeeId,
                        request.Contract.EmployeeTypeId,
                        request.Contract.ContractNumber,
                        request.Contract.StartDate,
                        request.Contract.EndDate,
                        request.Contract.Salary,
                        request.Contract.BucketName,
                        request.Contract.FileName
                    )
                );

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное обновление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> Delete([FromRoute] Guid employeeId, [FromRoute] Guid assignmentId, CancellationToken cancellationToken = default)
        {
            var command = new DeleteAssignmentCommand(employeeId, assignmentId);

            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>
                (
                    onSuccess: () => Ok("Успешное удаление!"),
                    onFailure: errors => BadRequest(errors)
                );
        }
    }
}
