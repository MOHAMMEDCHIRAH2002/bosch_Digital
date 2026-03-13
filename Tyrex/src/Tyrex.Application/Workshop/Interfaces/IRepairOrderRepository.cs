using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Interfaces;

public interface IRepairOrderRepository
{
    Task<RepairOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateNextOrderNumberAsync(CancellationToken cancellationToken = default);
    void Add(RepairOrder repairOrder);
}
