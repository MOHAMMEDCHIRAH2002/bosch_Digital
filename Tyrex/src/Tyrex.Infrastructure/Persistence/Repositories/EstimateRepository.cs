using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Domain.Commerce;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class EstimateRepository : IEstimateRepository
{
    private readonly ApplicationDbContext _context;
    public EstimateRepository(ApplicationDbContext context) => _context = context;

    public async Task<Estimate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<Estimate>().Include(e => e.Items).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Estimate?> GetActiveByRepairOrderIdAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<Estimate>()
            .Include(e => e.Items)
            .Where(e => e.RepairOrderId == repairOrderId)
            .OrderByDescending(e => e.Version)
            .FirstOrDefaultAsync(ct);

    public void Add(Estimate estimate) => _context.Set<Estimate>().Add(estimate);
}
