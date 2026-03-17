using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.Domain.Fleet;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Queries.GetRepairOrderById;

internal sealed class GetRepairOrderByIdQueryHandler : IQueryHandler<GetRepairOrderByIdQuery, RepairOrderDetailResponse>
{
    private readonly IApplicationDbContext _context;

    public GetRepairOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RepairOrderDetailResponse>> Handle(GetRepairOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var repairOrder = await _context.Set<RepairOrder>()
            .AsNoTracking()
            .FirstOrDefaultAsync(ro => ro.Id == request.Id, cancellationToken);

        if (repairOrder is null)
        {
            return Result.Failure<RepairOrderDetailResponse>(Error.NotFound("RepairOrder.NotFound", "Repair order not found."));
        }

        var customer = await _context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == repairOrder.CustomerId, cancellationToken);

        var vehicle = await _context.Set<Vehicle>()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == repairOrder.VehicleId, cancellationToken);

        var response = new RepairOrderDetailResponse(
            repairOrder.Id,
            repairOrder.OrderNumber,
            repairOrder.CustomerId,
            customer is not null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
            customer?.Email ?? "Unknown",
            customer?.Phone ?? "Unknown",
            repairOrder.VehicleId,
            vehicle is not null ? $"{vehicle.Make} {vehicle.Model}" : "Unknown",
            vehicle?.LicensePlate ?? "Unknown",
            vehicle?.Vin ?? "Unknown",
            repairOrder.Type.ToString(),
            repairOrder.Status.ToString(),
            repairOrder.VisitReason,
            repairOrder.IntakeMileage,
            repairOrder.IntakePhotoUrls.ToList(),
            repairOrder.CreatedOnUtc);

        return Result.Success(response);
    }
}
