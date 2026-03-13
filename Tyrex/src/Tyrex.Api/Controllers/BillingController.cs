using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Billing.Commands.GenerateInvoice;
using Tyrex.Application.Billing.Commands.RegisterPayment;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly ISender _sender;

    public BillingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("invoices")]
    public async Task<IActionResult> GenerateInvoice([FromBody] GenerateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GenerateInvoice), new { id = result.Value }, result.Value);
    }

    [HttpPost("invoices/{id:guid}/pay")]
    public async Task<IActionResult> RegisterPayment(Guid id, [FromBody] RegisterPaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterPaymentCommand(id, request.Amount, request.Method, request.ReferenceInfo);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpGet("invoices/{id:guid}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(Guid id, CancellationToken cancellationToken)
    {
        var query = new Tyrex.Application.Billing.Queries.GetInvoicePdf.GetInvoicePdfQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return File(result.Value, "application/pdf", $"Invoice_{id}.pdf");
    }
}

public record RegisterPaymentRequest(
    decimal Amount,
    Domain.Billing.PaymentMethod Method,
    string? ReferenceInfo);
