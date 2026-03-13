using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Workshop.Commands.SubmitDiagnosis;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/repair-orders/{repairOrderId:guid}/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly ISender _sender;

    public DiagnosticsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitDiagnosis(Guid repairOrderId, [FromBody] SubmitDiagnosisRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitDiagnosisCommand(
            repairOrderId,
            request.TechnicianId,
            request.Notes,
            request.MediaUrls ?? new List<string>());

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(SubmitDiagnosis), new { repairOrderId, id = result.Value }, result.Value);
    }
}

public record SubmitDiagnosisRequest(
    Guid TechnicianId,
    string Notes,
    List<string>? MediaUrls);
