using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Fleet.Interfaces;
using Tyrex.Domain.Fleet;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class VehicleRepository : IVehicleRepository
{
    private readonly ApplicationDbContext _context;
    public VehicleRepository(ApplicationDbContext context) => _context = context;

    public async Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<Vehicle>().FirstOrDefaultAsync(v => v.Id == id, ct);

    public void Add(Vehicle vehicle) => _context.Set<Vehicle>().Add(vehicle);
}
