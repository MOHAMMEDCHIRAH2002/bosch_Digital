using System.Net;
using System.Net.Http.Json;

namespace Tyrex.IntegrationTests;

public class VehicleEndpointsTests : BaseIntegrationTest
{
    private Guid _customerId;

    public VehicleEndpointsTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _customerId = await CreateTestCustomerAsync();
    }

    private async Task<Guid> CreateTestCustomerAsync()
    {
        var request = new
        {
            firstName = "Vehicle",
            lastName = "Owner",
            email = $"vehicle.owner.{Guid.NewGuid()}@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        var response = await Client.PostAsJsonAsync("/api/customers", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    [Fact]
    public async Task CreateVehicle_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            vin = "VF7LA9HXG12345678",
            licensePlate = "AB-123-CD",
            make = "Peugeot",
            model = "3008",
            year = 2020,
            customerId = _customerId,
            isInternalFleet = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var vehicleId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, vehicleId);
    }

    [Fact]
    public async Task CreateVehicle_WithInvalidCustomer_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            vin = "VF7LA9HXG12345679",
            licensePlate = "AB-123-CE",
            make = "Peugeot",
            model = "3008",
            year = 2020,
            customerId = Guid.NewGuid(),
            isInternalFleet = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateVehicle_WithDuplicateVin_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            vin = "VF7LA9HXG12345680",
            licensePlate = "AB-123-CF",
            make = "Peugeot",
            model = "3008",
            year = 2020,
            customerId = _customerId,
            isInternalFleet = false
        };

        // Create first vehicle
        var firstResponse = await Client.PostAsJsonAsync("/api/vehicles", request);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        // Create second vehicle with same VIN but different plate
        var secondRequest = new
        {
            vin = "VF7LA9HXG12345680",
            licensePlate = "AB-123-CG",
            make = "Peugeot",
            model = "3008",
            year = 2020,
            customerId = _customerId,
            isInternalFleet = false
        };

        // Act
        var secondResponse = await Client.PostAsJsonAsync("/api/vehicles", secondRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task GetVehicles_ReturnsList()
    {
        // Arrange - Create a vehicle
        var createRequest = new
        {
            vin = "VF1R9800X12345679",
            licensePlate = "XY-999-ZZ",
            make = "Renault",
            model = "Clio",
            year = 2022,
            customerId = _customerId,
            isInternalFleet = false
        };
        await Client.PostAsJsonAsync("/api/vehicles", createRequest);

        // Act
        var response = await Client.GetAsync("/api/vehicles");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<VehicleResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetVehicles_WithCustomerFilter_ReturnsFilteredResults()
    {
        // Arrange - Create a vehicle
        var createRequest = new
        {
            vin = "WVWZZZ1KZ12345680",
            licensePlate = "TEST-123",
            make = "Volkswagen",
            model = "Transporter",
            year = 2019,
            customerId = _customerId,
            isInternalFleet = false
        };
        await Client.PostAsJsonAsync("/api/vehicles", createRequest);

        // Act
        var response = await Client.GetAsync($"/api/vehicles?customerId={_customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedList<VehicleResponse>>(JsonOptions);
        Assert.NotNull(result);
        Assert.All(result.Items, v => Assert.Equal(_customerId, v.CustomerId));
    }

    private class PagedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    private class VehicleResponse
    {
        public Guid Id { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsInternalFleet { get; set; }
    }
}
