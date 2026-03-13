using Microsoft.EntityFrameworkCore;
using Tyrex.Infrastructure.Persistence.Interceptors;
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
        
        // Seed initial admin user
        modelBuilder.Entity<Tyrex.Domain.Identity.User>().HasData(new
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Email = "admin@tyrex.com",
            PasswordHash = "admin123", // MVP: simple cleartext for now
            FirstName = "Admin",
            LastName = "Tyrex",
            Role = Tyrex.Domain.Identity.Role.Admin
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            _auditableEntityInterceptor,
            _softDeletableEntityInterceptor);
        
        base.OnConfiguring(optionsBuilder);
    }
}
