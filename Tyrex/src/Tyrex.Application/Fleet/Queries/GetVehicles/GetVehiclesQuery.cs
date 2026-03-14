using Tyrex.Application.Messaging;

namespace Tyrex.Application.Fleet.Queries.GetVehicles;

public sealed record GetVehiclesQuery(Guid? CustomerId = null, string? SearchTerm = null, int Page = 1, int PageSize = 20) : IQuery<VehiclesListResponse>;

public sealed record VehiclesListResponse(
    List<VehicleDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public sealed record VehicleDto(
    Guid Id,
    string Vin,
    string LicensePlate,
    string Make,
    string Model,
    int Year,
    Guid CustomerId,
    bool IsInternalFleet,
    DateTime CreatedOnUtc
);
