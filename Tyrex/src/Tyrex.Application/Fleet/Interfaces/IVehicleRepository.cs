using Tyrex.Domain.Fleet;

namespace Tyrex.Application.Fleet.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Vehicle vehicle);
}
