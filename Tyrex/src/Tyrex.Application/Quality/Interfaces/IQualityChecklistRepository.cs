using Tyrex.Domain.Quality;

namespace Tyrex.Application.Quality.Interfaces;

public interface IQualityChecklistRepository
{
    Task<QualityChecklist?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QualityChecklist?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken cancellationToken = default);
    void Add(QualityChecklist checklist);
}
