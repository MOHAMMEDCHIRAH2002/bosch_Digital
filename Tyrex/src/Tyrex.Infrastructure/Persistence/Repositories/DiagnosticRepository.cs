using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class DiagnosticRepository : IDiagnosticRepository
{
    private readonly ApplicationDbContext _context;
    public DiagnosticRepository(ApplicationDbContext context) => _context = context;

    public async Task<Diagnostic?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<Diagnostic>().FirstOrDefaultAsync(d => d.Id == id, ct);

    public async Task<Diagnostic?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken ct = default)
        => await _context.Set<Diagnostic>().FirstOrDefaultAsync(d => d.RepairOrderId == repairOrderId, ct);

    public void Add(Diagnostic diagnostic) => _context.Set<Diagnostic>().Add(diagnostic);
}
