using Tyrex.Application.Messaging;

namespace Tyrex.Application.Reporting.Queries.GetDashboardKpis;

public sealed record GetDashboardKpisQuery() : IQuery<DashboardKpisResponse>;

public sealed record DashboardKpisResponse(
    int ActiveRepairOrders,
    int PendingEstimates,
    int VehiclesReadyForPickup,
    decimal TodayRevenue);
