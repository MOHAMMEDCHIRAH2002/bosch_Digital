using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class RepairOrderRepository : IRepairOrderRepository
{
    private readonly ApplicationDbContext _context;
    public RepairOrderRepository(ApplicationDbContext context) => _context = context;

    public async Task<RepairOrder?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<RepairOrder>().FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<string> GenerateNextOrderNumberAsync(CancellationToken ct = default)
    {
        var count = await _context.Set<RepairOrder>().CountAsync(ct);
        return $"OR-{(count + 1):D6}";
    }

    public void Add(RepairOrder repairOrder) => _context.Set<RepairOrder>().Add(repairOrder);
}
