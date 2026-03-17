using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.Domain.Fleet;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Fleet.Queries.GetVehicleById;

internal sealed class GetVehicleByIdQueryHandler : IQueryHandler<GetVehicleByIdQuery, VehicleDetailResponse>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehicleDetailResponse>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Set<Vehicle>()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);

        if (vehicle is null)
        {
            return Result.Failure<VehicleDetailResponse>(Error.NotFound("Vehicle.NotFound", "Vehicle not found."));
        }

        var customer = await _context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == vehicle.CustomerId, cancellationToken);

        var response = new VehicleDetailResponse(
            vehicle.Id,
            vehicle.Vin,
            vehicle.LicensePlate,
            vehicle.Make,
            vehicle.Model,
            vehicle.Year,
            vehicle.CustomerId,
            customer is not null ? $"{customer.FirstName} {customer.LastName}" : "Unknown",
            vehicle.IsInternalFleet,
            vehicle.CreatedOnUtc);

        return Result.Success(response);
    }
}
