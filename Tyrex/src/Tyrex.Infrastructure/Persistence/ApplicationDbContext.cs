using Microsoft.EntityFrameworkCore;
using Tyrex.Infrastructure.Persistence.Interceptors;
using Tyrex.Infrastructure.Persistence.Seeding;
using Tyrex.SharedKernel.Primitives;

using Tyrex.Application.Interfaces;

namespace Tyrex.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;
    private readonly SoftDeletableEntityInterceptor _softDeletableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor,
        SoftDeletableEntityInterceptor softDeletableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
        _softDeletableEntityInterceptor = softDeletableEntityInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply seed data
        modelBuilder.ApplySeedData();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            _auditableEntityInterceptor,
            _softDeletableEntityInterceptor);

        base.OnConfiguring(optionsBuilder);
    }

    public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
