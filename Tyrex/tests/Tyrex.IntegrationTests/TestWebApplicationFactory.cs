using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tyrex.Infrastructure.Persistence;

namespace Tyrex.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection _connection;

    public TestWebApplicationFactory()
    {
        // Create and open a shared connection that will be kept alive throughout the tests
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Add test configuration for JWT settings
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "TestSecretKeyThatIsAtLeast32CharactersLong!",
                ["Jwt:Issuer"] = "Tyrex.Test",
                ["Jwt:Audience"] = "Tyrex.Test",
                ["Jwt:ExpirationMinutes"] = "60"
            };
            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices((context, services) =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory SQLite database using the shared connection
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });

        builder.UseEnvironment("Testing");
    }

    /// <summary>
    /// Creates and seeds the database. Call this once in test class constructors.
    /// </summary>
    public void InitializeDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
    }

    public new void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
