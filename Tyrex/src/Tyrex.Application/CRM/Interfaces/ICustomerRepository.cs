using Tyrex.Domain.CRM;

namespace Tyrex.Application.CRM.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Customer customer);
}
