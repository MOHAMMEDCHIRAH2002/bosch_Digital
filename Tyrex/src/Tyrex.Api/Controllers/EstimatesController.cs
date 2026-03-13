using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Commerce.Commands.GenerateEstimate;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/estimates")]
public class EstimatesController : ControllerBase
{
    private readonly ISender _sender;

    public EstimatesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateEstimate([FromBody] GenerateEstimateCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GenerateEstimate), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveEstimate(Guid id, [FromBody] ApproveEstimateRequest request, CancellationToken cancellationToken)
    {
        var command = new Tyrex.Application.Commerce.Commands.ApproveEstimate.ApproveEstimateCommand(id, request.ClientApprovalProofUrl);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("{id:guid}/refuse")]
    public async Task<IActionResult> RefuseEstimate(Guid id, CancellationToken cancellationToken)
    {
        var command = new Tyrex.Application.Commerce.Commands.RefuseEstimate.RefuseEstimateCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetEstimatePdf(Guid id, CancellationToken cancellationToken)
    {
        var query = new Tyrex.Application.Commerce.Queries.GetEstimatePdf.GetEstimatePdfQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return File(result.Value, "application/pdf", $"Estimate_{id}.pdf");
    }

    [HttpPost("{id:guid}/send-email")]
    public async Task<IActionResult> SendEmail(Guid id, [FromBody] SendEstimateEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new Tyrex.Application.Commerce.Commands.SendEstimateEmail.SendEstimateEmailCommand(id, request.ToEmail);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }
}

public record ApproveEstimateRequest(string ClientApprovalProofUrl);
public record SendEstimateEmailRequest(string ToEmail);
