using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tyrex.Infrastructure.Persistence;

namespace Tyrex.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient Client;
    protected readonly JsonSerializerOptions JsonOptions;

    protected BaseIntegrationTest(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Factory.InitializeDatabase();
        Client = factory.CreateClient();
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    protected Task<ApplicationDbContext> GetDbContextAsync()
    {
        var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return Task.FromResult(dbContext);
    }

    protected async Task AuthenticateAsync()
    {
        // Login with default seeded user
        var loginRequest = new
        {
            email = "admin@tyrex.com",
            password = "admin123"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", result!.AccessToken);
    }

    public virtual Task InitializeAsync()
    {
        return AuthenticateAsync();
    }

    public virtual Task DisposeAsync()
    {
        // Clean up is handled by re-initializing the database in the next test
        // The shared connection persists but each test class gets its own factory instance
        return Task.CompletedTask;
    }

    private class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
