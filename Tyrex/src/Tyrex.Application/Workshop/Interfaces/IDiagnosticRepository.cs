using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Interfaces;

public interface IDiagnosticRepository
{
    Task<Diagnostic?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Diagnostic?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken cancellationToken = default);
    void Add(Diagnostic diagnostic);
}
