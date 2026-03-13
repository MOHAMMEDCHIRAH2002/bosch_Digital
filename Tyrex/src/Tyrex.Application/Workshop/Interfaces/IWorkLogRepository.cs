using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Interfaces;

public interface IWorkLogRepository
{
    Task<WorkLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkLog?> GetActiveLogForRepairOrderAsync(Guid repairOrderId, CancellationToken cancellationToken = default);
    void Add(WorkLog workLog);
}
