using Microsoft.EntityFrameworkCore;
using Tyrex.Domain.Billing;
using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
