using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class WorkLogRepository : IWorkLogRepository
{
    private readonly ApplicationDbContext _context;
    public WorkLogRepository(ApplicationDbContext context) => _context = context;

    public async Task<WorkLog?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<WorkLog>().FirstOrDefaultAsync(w => w.Id == id, ct);

    public async Task<WorkLog?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<WorkLog>().FirstOrDefaultAsync(w => w.RepairOrderId == repairOrderId, ct);

    public async Task<WorkLog?> GetActiveLogForRepairOrderAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<WorkLog>()
            .Where(w => w.RepairOrderId == repairOrderId && w.EndTime == null)
            .FirstOrDefaultAsync(ct);

    public void Add(WorkLog workLog) => _context.Set<WorkLog>().Add(workLog);
}
