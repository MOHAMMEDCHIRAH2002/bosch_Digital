using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Quality.Commands.SubmitQualityChecklist;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/repair-orders/{repairOrderId:guid}/quality")]
public class QualityController : ControllerBase
{
    private readonly ISender _sender;

    public QualityController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitChecklist(Guid repairOrderId, [FromBody] SubmitQualityChecklistRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitQualityChecklistCommand(
            repairOrderId,
            request.InspectorId,
            request.Items,
            request.FinalNotes);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { ChecklistId = result.Value });
    }
}

public record SubmitQualityChecklistRequest(
    Guid InspectorId,
    List<ChecklistItemResult> Items,
    string? FinalNotes);
