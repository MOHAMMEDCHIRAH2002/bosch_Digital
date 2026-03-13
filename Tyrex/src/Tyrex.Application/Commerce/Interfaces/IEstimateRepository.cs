using Tyrex.Domain.Commerce;

namespace Tyrex.Application.Commerce.Interfaces;

public interface IEstimateRepository
{
    Task<Estimate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Estimate?> GetActiveByRepairOrderIdAsync(Guid repairOrderId, CancellationToken cancellationToken = default);
    void Add(Estimate estimate);
}
