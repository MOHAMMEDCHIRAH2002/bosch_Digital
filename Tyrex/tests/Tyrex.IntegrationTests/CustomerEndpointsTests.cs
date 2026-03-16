using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tyrex.IntegrationTests;

public class CustomerEndpointsTests : BaseIntegrationTest
{
    public CustomerEndpointsTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateCustomer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
            email = "john.doe@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, customerId);
    }

    [Fact]
    public async Task CreateCustomer_CompanyType_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            firstName = "Jane",
            lastName = "Smith",
            email = "jane.smith@company.com",
            phone = "+33687654321",
            type = "Company",
            companyName = "Test Company SARL"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, customerId);
    }

    [Fact]
    public async Task CreateCustomer_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
            email = "invalid-email",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithEmptyFirstName_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            firstName = "",
            lastName = "Doe",
            email = "john.doe@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            firstName = "John",
            lastName = "Doe",
            email = "duplicate@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        // Create first customer
        var firstResponse = await Client.PostAsJsonAsync("/api/customers", request);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Act - Try to create second with same email
        var secondResponse = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_ReturnsList()
    {
        // Arrange - Create a customer first
        var createRequest = new
        {
            firstName = "Alice",
            lastName = "Wonder",
            email = "alice@test.com",
            phone = "+33611111111",
            type = "Individual",
            companyName = (string?)null
        };
        await Client.PostAsJsonAsync("/api/customers", createRequest);

        // Act
        var response = await Client.GetAsync("/api/customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<CustomerResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetCustomers_WithSearchFilter_ReturnsFilteredResults()
    {
        // Arrange - Create a customer
        var createRequest = new
        {
            firstName = "Bob",
            lastName = "Searchable",
            email = "bob.search@test.com",
            phone = "+33622222222",
            type = "Individual",
            companyName = (string?)null
        };
        await Client.PostAsJsonAsync("/api/customers", createRequest);

        // Act
        var response = await Client.GetAsync("/api/customers?search=Searchable");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<CustomerResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.Contains(result.Items, c => c.LastName == "Searchable");
    }

    private class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class CustomerResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
    }
}
