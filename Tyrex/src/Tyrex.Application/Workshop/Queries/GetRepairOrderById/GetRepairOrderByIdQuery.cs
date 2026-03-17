using Tyrex.Application.Messaging;
using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Queries.GetRepairOrderById;

public sealed record GetRepairOrderByIdQuery(Guid Id) : IQuery<RepairOrderDetailResponse>;

public sealed record RepairOrderDetailResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    Guid VehicleId,
    string VehicleInfo,
    string VehicleLicensePlate,
    string VehicleVin,
    string Type,
    string Status,
    string VisitReason,
    int? IntakeMileage,
    List<string> IntakePhotoUrls,
    DateTime CreatedOnUtc
);
