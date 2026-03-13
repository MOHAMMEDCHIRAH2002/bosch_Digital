using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Inventory.Interfaces;
using Tyrex.Domain.Inventory;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class StockItemRepository : IStockItemRepository
{
    private readonly ApplicationDbContext _context;
    public StockItemRepository(ApplicationDbContext context) => _context = context;

    public async Task<StockItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<StockItem>().FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<StockItem?> GetByPartNumberAsync(string partNumber, CancellationToken ct = default)
        => await _context.Set<StockItem>().FirstOrDefaultAsync(s => s.PartNumber == partNumber, ct);

    public void Add(StockItem stockItem) => _context.Set<StockItem>().Add(stockItem);
}
