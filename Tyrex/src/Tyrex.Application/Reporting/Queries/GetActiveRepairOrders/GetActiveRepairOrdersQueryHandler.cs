using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.Domain.Fleet;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Reporting.Queries.GetActiveRepairOrders;

internal sealed class GetActiveRepairOrdersQueryHandler : IQueryHandler<GetActiveRepairOrdersQuery, IReadOnlyList<ActiveRepairOrderResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveRepairOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<ActiveRepairOrderResponse>>> Handle(GetActiveRepairOrdersQuery request, CancellationToken cancellationToken)
    {
        var activeOrders = await (from ro in _context.Set<RepairOrder>()
                                  join c in _context.Set<Customer>() on ro.CustomerId equals c.Id
                                  join v in _context.Set<Vehicle>() on ro.VehicleId equals v.Id
                                  where ro.Status != RepairOrderStatus.Draft && 
                                        ro.Status != RepairOrderStatus.Delivered && 
                                        ro.Status != RepairOrderStatus.ClosedUnrepaired
                                  orderby ro.CreatedOnUtc descending
                                  select new ActiveRepairOrderResponse(
                                      ro.Id,
                                      ro.OrderNumber,
                                      c.Type == CustomerType.Individual ? $"{c.FirstName} {c.LastName}" : c.CompanyName ?? "Unknown Company",
                                      $"{v.Make} {v.Model} ({v.LicensePlate})",
                                      ro.Status.ToString(),
                                      ro.CreatedOnUtc
                                  )).ToListAsync(cancellationToken);

        return activeOrders;
    }
}
