using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Fleet;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Fleet.Queries.GetVehicles;

internal sealed class GetVehiclesQueryHandler : IQueryHandler<GetVehiclesQuery, VehiclesListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetVehiclesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VehiclesListResponse>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Set<Vehicle>().AsNoTracking().AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(v => v.CustomerId == request.CustomerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(v =>
                EF.Functions.Like(v.Vin.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(v.LicensePlate.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(v.Make.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(v.Model.ToLower(), $"%{searchLower}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(v => v.CreatedOnUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VehicleDto(
                v.Id,
                v.Vin,
                v.LicensePlate,
                v.Make,
                v.Model,
                v.Year,
                v.CustomerId,
                v.IsInternalFleet,
                v.CreatedOnUtc))
            .ToListAsync(cancellationToken);

        return new VehiclesListResponse(items, totalCount, request.Page, request.PageSize);
    }
}
