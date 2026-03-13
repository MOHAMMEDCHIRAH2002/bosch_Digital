using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Reporting.Queries.GetActiveRepairOrders;
using Tyrex.Application.Reporting.Queries.GetDashboardKpis;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDashboardKpisQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("active-orders")]
    public async Task<IActionResult> GetActiveRepairOrders(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetActiveRepairOrdersQuery(), cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
