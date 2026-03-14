using Tyrex.Application.Messaging;
using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Queries.GetRepairOrders;

public sealed record GetRepairOrdersQuery(
    RepairOrderStatus? Status = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20
) : IQuery<RepairOrdersListResponse>;

public sealed record RepairOrdersListResponse(
    List<RepairOrderDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public sealed record RepairOrderDto(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string CustomerName,
    Guid VehicleId,
    string VehicleInfo,
    string Type,
    string Status,
    string VisitReason,
    DateTime CreatedOnUtc
);
