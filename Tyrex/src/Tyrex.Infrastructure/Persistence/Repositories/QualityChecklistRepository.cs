using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Quality.Interfaces;
using Tyrex.Domain.Quality;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class QualityChecklistRepository : IQualityChecklistRepository
{
    private readonly ApplicationDbContext _context;
    public QualityChecklistRepository(ApplicationDbContext context) => _context = context;

    public async Task<QualityChecklist?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<QualityChecklist>()
            .Include(q => q.Items)
            .FirstOrDefaultAsync(q => q.Id == id, ct);

    public async Task<QualityChecklist?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<QualityChecklist>()
            .Include(q => q.Items)
            .FirstOrDefaultAsync(q => q.RepairOrderId == repairOrderId, ct);

    public void Add(QualityChecklist checklist) => _context.Set<QualityChecklist>().Add(checklist);
}
