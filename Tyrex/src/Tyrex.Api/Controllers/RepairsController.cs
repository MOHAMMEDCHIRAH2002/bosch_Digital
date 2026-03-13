using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Workshop.Commands.CompleteRepair;
using Tyrex.Application.Workshop.Commands.StartRepair;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/repairs")]
public class RepairsController : ControllerBase
{
    private readonly ISender _sender;

    public RepairsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartRepair(Guid id, [FromBody] StarRepairRequest request, CancellationToken cancellationToken)
    {
        var command = new StartRepairCommand(id, request.TechnicianId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteRepair(Guid id, [FromBody] CompleteRepairRequest request, CancellationToken cancellationToken)
    {
        var command = new CompleteRepairCommand(id, request.TechnicianId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}

public record StarRepairRequest(Guid TechnicianId);
public record CompleteRepairRequest(Guid TechnicianId);
