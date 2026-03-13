using Tyrex.Domain.Billing;

namespace Tyrex.Application.Billing.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByRepairOrderIdAsync(Guid repairOrderId, CancellationToken cancellationToken = default);
    Task<string> GenerateNextInvoiceNumberAsync(CancellationToken cancellationToken = default);
    void Add(Invoice invoice);
}
