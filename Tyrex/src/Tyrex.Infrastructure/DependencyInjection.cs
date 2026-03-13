using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tyrex.Application.Interfaces;
using Tyrex.Infrastructure.Authentication;
using Tyrex.Infrastructure.Persistence;
using Tyrex.Infrastructure.Persistence.Interceptors;
using Tyrex.SharedKernel.Interfaces;

namespace Tyrex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton<AuditableEntityInterceptor>();
        services.AddSingleton<SoftDeletableEntityInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            // For MVP/E2E testing, use SQLite if we want to ensure it works locally
            options.UseSqlite("Data Source=Tyrex.db");
            
            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<SoftDeletableEntityInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(sp => 
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        // Repository registrations
        services.AddScoped<Tyrex.Application.Workshop.Interfaces.IRepairOrderRepository, Tyrex.Infrastructure.Persistence.Repositories.RepairOrderRepository>();
        services.AddScoped<Tyrex.Application.CRM.Interfaces.ICustomerRepository, Tyrex.Infrastructure.Persistence.Repositories.CustomerRepository>();
        services.AddScoped<Tyrex.Application.Fleet.Interfaces.IVehicleRepository, Tyrex.Infrastructure.Persistence.Repositories.VehicleRepository>();
        services.AddScoped<Tyrex.Application.Workshop.Interfaces.IDiagnosticRepository, Tyrex.Infrastructure.Persistence.Repositories.DiagnosticRepository>();
        services.AddScoped<Tyrex.Application.Workshop.Interfaces.IWorkLogRepository, Tyrex.Infrastructure.Persistence.Repositories.WorkLogRepository>();
        services.AddScoped<Tyrex.Application.Commerce.Interfaces.IEstimateRepository, Tyrex.Infrastructure.Persistence.Repositories.EstimateRepository>();
        services.AddScoped<Tyrex.Application.Inventory.Interfaces.IStockItemRepository, Tyrex.Infrastructure.Persistence.Repositories.StockItemRepository>();
        services.AddScoped<Tyrex.Application.Billing.Interfaces.IInvoiceRepository, Tyrex.Infrastructure.Persistence.Repositories.InvoiceRepository>();
        services.AddScoped<Tyrex.Application.Quality.Interfaces.IQualityChecklistRepository, Tyrex.Infrastructure.Persistence.Repositories.QualityChecklistRepository>();
        services.AddScoped<Tyrex.Application.Identity.Interfaces.IUserRepository, Tyrex.Infrastructure.Persistence.Repositories.UserRepository>();
            
        // MVP Services
        services.AddTransient<Tyrex.Application.Interfaces.IEmailService, Tyrex.Infrastructure.Services.MockEmailService>();
        services.AddTransient<Tyrex.Application.Interfaces.IPdfService, Tyrex.Infrastructure.Services.MockPdfService>();

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
                };
            });
            
        services.AddAuthorization();

        return services;
    }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
