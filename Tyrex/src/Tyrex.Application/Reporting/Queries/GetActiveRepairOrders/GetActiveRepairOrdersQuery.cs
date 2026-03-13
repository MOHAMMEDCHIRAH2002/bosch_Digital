using Tyrex.Application.Messaging;

namespace Tyrex.Application.Reporting.Queries.GetActiveRepairOrders;

public sealed record GetActiveRepairOrdersQuery() : IQuery<IReadOnlyList<ActiveRepairOrderResponse>>;

public sealed record ActiveRepairOrderResponse(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    string VehicleDetails,
    string Status,
    DateTime CreatedAtUtc);
