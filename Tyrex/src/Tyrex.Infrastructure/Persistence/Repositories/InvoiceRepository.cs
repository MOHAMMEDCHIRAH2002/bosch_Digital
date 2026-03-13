using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Billing.Interfaces;
using Tyrex.Domain.Billing;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;
    public InvoiceRepository(ApplicationDbContext context) => _context = context;

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<Invoice>()
            .Include(i => i.Lines)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<Invoice?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<Invoice>()
            .Include(i => i.Lines)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.RepairOrderId == repairOrderId, ct);

    public async Task<string> GenerateNextInvoiceNumberAsync(CancellationToken ct = default)
    {
        var count = await _context.Set<Invoice>().CountAsync(ct);
        return $"INV-{DateTime.UtcNow:yyyyMM}-{(count + 1):D4}";
    }

    public void Add(Invoice invoice) => _context.Set<Invoice>().Add(invoice);
}
