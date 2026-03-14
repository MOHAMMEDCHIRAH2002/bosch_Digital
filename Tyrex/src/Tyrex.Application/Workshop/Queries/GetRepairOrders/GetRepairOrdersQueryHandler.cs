using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.Domain.Fleet;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Queries.GetRepairOrders;

internal sealed class GetRepairOrdersQueryHandler : IQueryHandler<GetRepairOrdersQuery, RepairOrdersListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetRepairOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<RepairOrdersListResponse>> Handle(GetRepairOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Set<RepairOrder>().AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(ro => ro.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(ro =>
                EF.Functions.Like(ro.OrderNumber.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(ro.VisitReason.ToLower(), $"%{searchLower}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var repairOrders = await query
            .OrderByDescending(ro => ro.CreatedOnUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var customerIds = repairOrders.Select(ro => ro.CustomerId).Distinct().ToList();
        var vehicleIds = repairOrders.Select(ro => ro.VehicleId).Distinct().ToList();

        var customers = await _context.Set<Customer>()
            .AsNoTracking()
            .Where(c => customerIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c, cancellationToken);

        var vehicles = await _context.Set<Vehicle>()
            .AsNoTracking()
            .Where(v => vehicleIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id, v => v, cancellationToken);

        var items = repairOrders.Select(ro =>
        {
            var customer = customers.GetValueOrDefault(ro.CustomerId);
            var vehicle = vehicles.GetValueOrDefault(ro.VehicleId);

            return new RepairOrderDto(
                ro.Id,
                ro.OrderNumber,
                ro.CustomerId,
                customer is not null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
                ro.VehicleId,
                vehicle is not null ? $"{vehicle.Make} {vehicle.Model} ({vehicle.LicensePlate})" : "Unknown",
                ro.Type.ToString(),
                ro.Status.ToString(),
                ro.VisitReason,
                ro.CreatedOnUtc);
        }).ToList();

        return new RepairOrdersListResponse(items, totalCount, request.Page, request.PageSize);
    }
}
