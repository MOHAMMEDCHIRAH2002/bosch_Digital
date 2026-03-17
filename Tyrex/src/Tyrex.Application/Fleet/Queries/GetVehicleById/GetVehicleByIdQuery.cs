using Tyrex.Application.Messaging;

namespace Tyrex.Application.Fleet.Queries.GetVehicleById;

public sealed record GetVehicleByIdQuery(Guid Id) : IQuery<VehicleDetailResponse>;

public sealed record VehicleDetailResponse(
    Guid Id,
    string Vin,
    string LicensePlate,
    string Make,
    string Model,
    int Year,
    Guid CustomerId,
    string CustomerName,
    bool IsInternalFleet,
    DateTime CreatedOnUtc
);
