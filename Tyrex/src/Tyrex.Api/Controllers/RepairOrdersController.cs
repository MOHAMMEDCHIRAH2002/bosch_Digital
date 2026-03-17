using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tyrex.Application.Workshop.Commands.AddIntakePhotos;
using Tyrex.Application.Workshop.Commands.CreateRepairOrder;
using Tyrex.Application.Workshop.Queries.GetRepairOrderById;
using Tyrex.Application.Workshop.Queries.GetRepairOrders;
using Tyrex.Domain.Workshop;

namespace Tyrex.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RepairOrdersController : ControllerBase
{
    private readonly ISender _sender;

    public RepairOrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetRepairOrders(
        [FromQuery] RepairOrderStatus? status,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRepairOrdersQuery(status, searchTerm, page, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRepairOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRepairOrderByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code == "RepairOrder.NotFound" ? NotFound(result.Error) : BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRepairOrder([FromBody] CreateRepairOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetRepairOrderById), new { id = result.Value }, result.Value);
    }

    [HttpPost("{id:guid}/photos")]
    public async Task<IActionResult> AddIntakePhotos(Guid id, [FromBody] List<string> photoUrls, CancellationToken cancellationToken)
    {
        var command = new AddIntakePhotosCommand(id, photoUrls);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code == "RepairOrder.NotFound" ? NotFound(result.Error) : BadRequest(result.Error);
        }

        return Ok(new { message = "Photos added successfully" });
    }
}
