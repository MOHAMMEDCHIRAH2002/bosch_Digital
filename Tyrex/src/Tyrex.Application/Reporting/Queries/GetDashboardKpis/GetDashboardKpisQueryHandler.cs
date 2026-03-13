using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Billing;
using Tyrex.Domain.Commerce;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Reporting.Queries.GetDashboardKpis;

internal sealed class GetDashboardKpisQueryHandler : IQueryHandler<GetDashboardKpisQuery, DashboardKpisResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetDashboardKpisQueryHandler(IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<DashboardKpisResponse>> Handle(GetDashboardKpisQuery request, CancellationToken cancellationToken)
    {
        // 1. Active Repair Orders (Not Draft, Not Closed, Not Delivered)
        var activeOrs = await _context.Set<RepairOrder>()
            .CountAsync(ro => 
                ro.Status != RepairOrderStatus.Draft && 
                ro.Status != RepairOrderStatus.Delivered && 
                ro.Status != RepairOrderStatus.ClosedUnrepaired, 
                cancellationToken);

        // 2. Pending Estimates (Awaiting Customer Approval)
        var pendingEstimates = await _context.Set<RepairOrder>()
            .CountAsync(ro => ro.Status == RepairOrderStatus.AwaitingCustomerApproval, cancellationToken);

        // 3. Vehicles Ready For Pickup (Repair Completed or Quality Validated or Invoiced, but not Delivered)
        var readyForPickup = await _context.Set<RepairOrder>()
            .CountAsync(ro => 
                ro.Status == RepairOrderStatus.RepairCompleted || 
                ro.Status == RepairOrderStatus.QualityValidated ||
                ro.Status == RepairOrderStatus.Invoiced, 
                cancellationToken);

        // 4. Today's Revenue (Sum of payments made today)
        var today = _dateTimeProvider.UtcNow.Date;
        var todayRevenue = await _context.Set<Payment>()
            .Where(p => p.PaymentDate >= today && p.PaymentDate < today.AddDays(1))
            .SumAsync(p => p.Amount, cancellationToken);

        return new DashboardKpisResponse(
            activeOrs,
            pendingEstimates,
            readyForPickup,
            todayRevenue);
    }
}
