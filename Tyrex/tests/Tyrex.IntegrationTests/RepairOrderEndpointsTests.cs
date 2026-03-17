using System.Net;
using System.Net.Http.Json;

namespace Tyrex.IntegrationTests;

public class RepairOrderEndpointsTests : BaseIntegrationTest
{
    private Guid _customerId;
    private Guid _vehicleId;

    public RepairOrderEndpointsTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _customerId = await CreateTestCustomerAsync();
        _vehicleId = await CreateTestVehicleAsync(_customerId);
    }

    private async Task<Guid> CreateTestCustomerAsync()
    {
        var request = new
        {
            firstName = "Repair",
            lastName = "Customer",
            email = $"repair.customer.{Guid.NewGuid()}@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        var response = await Client.PostAsJsonAsync("/api/customers", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateTestVehicleAsync(Guid customerId)
    {
        var request = new
        {
            vin = $"VIN{Guid.NewGuid().ToString("N")[..13]}",
            licensePlate = $"XX-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            make = "TestMake",
            model = "TestModel",
            year = 2020,
            customerId = customerId,
            isInternalFleet = false
        };

        var response = await Client.PostAsJsonAsync("/api/vehicles", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task CreateRepairOrder_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            customerId = _customerId,
            vehicleId = _vehicleId,
            type = "General",
            description = "Test repair order",
            mileage = 50000
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var repairOrderId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, repairOrderId);
    }

    [Fact]
    public async Task CreateRepairOrder_WithInvalidCustomer_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = Guid.NewGuid(),
            vehicleId = _vehicleId,
            type = "General",
            description = "Test repair order",
            mileage = 50000
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRepairOrder_WithInvalidVehicle_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            customerId = _customerId,
            vehicleId = Guid.NewGuid(),
            type = "General",
            description = "Test repair order",
            mileage = 50000
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRepairOrder_WithAllTypes_ReturnsCreated()
    {
        var types = new[] { "Sinistre", "General", "ServiceRapide", "RetourTechnique" };

        foreach (var type in types)
        {
            // Arrange
            var vehicleId = await CreateTestVehicleAsync(_customerId);
            var request = new
            {
                customerId = _customerId,
                vehicleId = vehicleId,
                type = type,
                description = $"Test {type} repair order",
                mileage = 50000
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/repairorders", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetRepairOrders_ReturnsList()
    {
        // Arrange - Create a repair order
        var createRequest = new
        {
            customerId = _customerId,
            vehicleId = _vehicleId,
            type = "General",
            description = "Test repair order for listing",
            mileage = 50000
        };
        await Client.PostAsJsonAsync("/api/repairorders", createRequest);

        // Act
        var response = await Client.GetAsync("/api/repairorders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<RepairOrderResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetRepairOrders_WithStatusFilter_ReturnsFilteredResults()
    {
        // Arrange - Create a repair order
        var createRequest = new
        {
            customerId = _customerId,
            vehicleId = _vehicleId,
            type = "General",
            description = "Test repair order with status",
            mileage = 50000
        };
        await Client.PostAsJsonAsync("/api/repairorders", createRequest);

        // Act - Filter by Draft status (new orders are created as Draft)
        var response = await Client.GetAsync("/api/repairorders?status=Draft");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<RepairOrderResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.All(result.Items, ro => Assert.Equal("Draft", ro.Status));
    }

    private class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class RepairOrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid VehicleId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Mileage { get; set; }
    }
}
