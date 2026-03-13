using Microsoft.EntityFrameworkCore;
using Tyrex.Application.CRM.Interfaces;
using Tyrex.Domain.CRM;
using Tyrex.Infrastructure.Persistence;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _context;
    public CustomerRepository(ApplicationDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<Customer>().FirstOrDefaultAsync(c => c.Id == id, ct);

    public void Add(Customer customer) => _context.Set<Customer>().Add(customer);
}
