using Tyrex.Domain.Inventory;

namespace Tyrex.Application.Inventory.Interfaces;

public interface IStockItemRepository
{
    Task<StockItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StockItem?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default);
    void Add(StockItem stockItem);
}
