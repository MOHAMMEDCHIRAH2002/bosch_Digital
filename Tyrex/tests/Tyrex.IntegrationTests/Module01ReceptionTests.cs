using System.Net;
using System.Net.Http.Json;

namespace Tyrex.IntegrationTests;

/// <summary>
/// Module 01 - Reception / Vehicle Intake Integration Tests
/// Tests the complete flow: Customer → Vehicle → RepairOrder
/// </summary>
public class Module01ReceptionTests : BaseIntegrationTest
{
    public Module01ReceptionTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Customer Tests

    [Fact]
    public async Task CreateCustomer_Individual_Success()
    {
        // Arrange
        var request = new
        {
            firstName = "Jean",
            lastName = "Dupont",
            email = $"jean.dupont_{Guid.NewGuid():N}@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
        var customerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, customerId);
    }

    [Fact]
    public async Task CreateCustomer_Company_Success()
    {
        // Arrange
        var request = new
        {
            firstName = "Marie",
            lastName = "Martin",
            email = $"contact_{Guid.NewGuid():N}@bosch-service.fr",
            phone = "+33123456789",
            type = "Company",
            companyName = "Bosch Car Service Paris"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/customers", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
        var customerId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, customerId);
    }

    [Fact]
    public async Task GetCustomerById_ExistingCustomer_ReturnsCustomer()
    {
        // Arrange - Create a customer first
        var createRequest = new
        {
            firstName = "Pierre",
            lastName = "Bernard",
            email = $"pierre.bernard_{Guid.NewGuid():N}@test.com",
            phone = "+33698765432",
            type = "Individual",
            companyName = (string?)null
        };
        var createResponse = await Client.PostAsJsonAsync("/api/customers", createRequest);
        var customerId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Act
        var response = await Client.GetAsync($"/api/customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDetailResponse>(JsonOptions);
        Assert.NotNull(customer);
        Assert.Equal(customerId, customer!.Id);
        Assert.Equal("Pierre", customer.FirstName);
        Assert.Equal("Bernard", customer.LastName);
        Assert.Equal("Individual", customer.Type);
    }

    [Fact]
    public async Task GetCustomerById_NonExisting_ReturnsNotFound()
    {
        // Act
        var response = await Client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomers_WithSearchTerm_ReturnsFilteredResults()
    {
        // Arrange - Create customers with unique names
        var uniqueName = $"TestSearch_{Guid.NewGuid():N}";
        var createResponse = await Client.PostAsJsonAsync("/api/customers", new
        {
            firstName = uniqueName,
            lastName = "Test",
            email = $"{uniqueName}@test.com",
            phone = "+33611111111",
            type = "Individual",
            companyName = (string?)null
        });

        // Act
        var response = await Client.GetAsync($"/api/customers?searchTerm={uniqueName}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CustomersListResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.Contains(result!.Items, c => c.FirstName == uniqueName);
    }

    #endregion

    #region Vehicle Tests

    [Fact]
    public async Task CreateVehicle_WithValidCustomer_Success()
    {
        // Arrange - Create a customer first
        var customerId = await CreateTestCustomerAsync();

        var request = new
        {
            vin = $"VIN{Guid.NewGuid():N}"[..17],
            licensePlate = $"AB-123-{Guid.NewGuid():N}"[..6],
            make = "Renault",
            model = "Clio",
            year = 2020,
            customerId = customerId,
            isInternalFleet = false
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
        var vehicleId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, vehicleId);
    }

    [Fact]
    public async Task CreateVehicle_InternalFleet_Success()
    {
        // Arrange - Create a customer first
        var customerId = await CreateTestCustomerAsync();

        var request = new
        {
            vin = $"VIN{Guid.NewGuid():N}"[..17],
            licensePlate = $"INT-{Guid.NewGuid():N}"[..6],
            make = "Peugeot",
            model = "308",
            year = 2022,
            customerId = customerId,
            isInternalFleet = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/vehicles", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
        var vehicleId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, vehicleId);
    }

    [Fact]
    public async Task GetVehicleById_ExistingVehicle_ReturnsVehicle()
    {
        // Arrange - Create customer and vehicle
        var customerId = await CreateTestCustomerAsync();
        var vehicleId = await CreateTestVehicleAsync(customerId);

        // Act
        var response = await Client.GetAsync($"/api/vehicles/{vehicleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var vehicle = await response.Content.ReadFromJsonAsync<VehicleDetailResponse>(JsonOptions);
        Assert.NotNull(vehicle);
        Assert.Equal(vehicleId, vehicle!.Id);
        Assert.Equal("TestMake", vehicle.Make);
        Assert.Equal(customerId, vehicle.CustomerId);
    }

    [Fact]
    public async Task GetVehicles_ByCustomerId_ReturnsOnlyCustomerVehicles()
    {
        // Arrange
        var customer1Id = await CreateTestCustomerAsync("customer1@test.com");
        var customer2Id = await CreateTestCustomerAsync("customer2@test.com");
        var vehicle1Id = await CreateTestVehicleAsync(customer1Id, "PLATE1");
        var vehicle2Id = await CreateTestVehicleAsync(customer2Id, "PLATE2");

        // Act
        var response = await Client.GetAsync($"/api/vehicles?customerId={customer1Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<VehiclesListResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.Single(result!.Items.Where(v => v.LicensePlate.Contains("PLATE1")));
        Assert.DoesNotContain(result.Items, v => v.LicensePlate.Contains("PLATE2"));
    }

    #endregion

    #region Repair Order Tests

    [Theory]
    [InlineData("General")]
    [InlineData("ServiceRapide")]
    [InlineData("RetourTechnique")]
    [InlineData("Sinistre")]
    public async Task CreateRepairOrder_AllTypes_Success(string type)
    {
        // Arrange
        var customerId = await CreateTestCustomerAsync();
        var vehicleId = await CreateTestVehicleAsync(customerId);

        var request = new Dictionary<string, object>
        {
            ["customerId"] = customerId,
            ["vehicleId"] = vehicleId,
            ["type"] = type,
            ["visitReason"] = $"Test {type} repair order",
            ["intakeMileage"] = 50000
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Debug: print response if not successful
        if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"DEBUG: Status={response.StatusCode}, Content={errorContent}");
        }

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
        var repairOrderId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, repairOrderId);

        // Verify the created OR
        var getResponse = await Client.GetAsync($"/api/repairorders/{repairOrderId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var repairOrder = await getResponse.Content.ReadFromJsonAsync<RepairOrderDetailResponse>(JsonOptions);
        Assert.NotNull(repairOrder);
        Assert.Equal(type, repairOrder!.Type);
        Assert.Equal(50000, repairOrder.IntakeMileage);
    }

    [Fact]
    public async Task CreateRepairOrder_WithoutMileage_Success()
    {
        // Arrange
        var customerId = await CreateTestCustomerAsync();
        var vehicleId = await CreateTestVehicleAsync(customerId);

        var request = new
        {
            customerId = customerId,
            vehicleId = vehicleId,
            type = "General",
            visitReason = "Test without mileage",
            intakeMileage = (int?)null
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {response.StatusCode}");
    }

    [Fact]
    public async Task CreateRepairOrder_InvalidCustomer_ReturnsBadRequest()
    {
        // Arrange
        var request = new Dictionary<string, object>
        {
            ["customerId"] = Guid.NewGuid(),
            ["vehicleId"] = Guid.NewGuid(),
            ["type"] = "General",
            ["visitReason"] = "Test repair order",
            ["intakeMileage"] = 50000
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/repairorders", request);

        // Assert - API returns 400 BadRequest when customer or vehicle is not found
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetRepairOrderById_ExistingOR_ReturnsCompleteDetails()
    {
        // Arrange
        var customerId = await CreateTestCustomerAsync();
        var vehicleId = await CreateTestVehicleAsync(customerId);
        var orId = await CreateTestRepairOrderAsync(customerId, vehicleId);

        // Act
        var response = await Client.GetAsync($"/api/repairorders/{orId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var repairOrder = await response.Content.ReadFromJsonAsync<RepairOrderDetailResponse>(JsonOptions);
        Assert.NotNull(repairOrder);
        Assert.Equal(orId, repairOrder!.Id);
        Assert.Equal(customerId, repairOrder.CustomerId);
        Assert.Equal(vehicleId, repairOrder.VehicleId);
        Assert.False(string.IsNullOrEmpty(repairOrder.CustomerName));
        Assert.False(string.IsNullOrEmpty(repairOrder.VehicleInfo));
    }

    [Fact]
    public async Task GetRepairOrders_WithStatusFilter_ReturnsFilteredResults()
    {
        // Arrange
        var customerId = await CreateTestCustomerAsync();
        var vehicleId = await CreateTestVehicleAsync(customerId);
        var orId = await CreateTestRepairOrderAsync(customerId, vehicleId);

        // Act
        var response = await Client.GetAsync("/api/repairorders?status=Draft");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RepairOrdersListResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.Contains(result!.Items, ro => ro.Id == orId);
    }

    #endregion

    #region End-to-End Flow Tests

    [Fact]
    public async Task CompleteReceptionFlow_CreateCustomerVehicleAndOR_Success()
    {
        // Step 1: Create Customer
        var customerRequest = new
        {
            firstName = "Sophie",
            lastName = "Laurent",
            email = $"sophie.laurent_{Guid.NewGuid():N}@test.com",
            phone = "+33655555555",
            type = "Individual",
            companyName = (string?)null
        };
        var customerResponse = await Client.PostAsJsonAsync("/api/customers", customerRequest);
        Assert.True(customerResponse.StatusCode == HttpStatusCode.Created || customerResponse.StatusCode == HttpStatusCode.OK,
            $"Customer creation failed with status: {customerResponse.StatusCode}");
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        // Step 2: Create Vehicle for Customer
        var vehicleRequest = new
        {
            vin = $"VF1LA0H5323{Guid.NewGuid():N}"[..17],
            licensePlate = $"AA-123-{Guid.NewGuid():N}"[..6],
            make = "Peugeot",
            model = "208",
            year = 2021,
            customerId = customerId,
            isInternalFleet = false
        };
        var vehicleResponse = await Client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        Assert.True(vehicleResponse.StatusCode == HttpStatusCode.Created || vehicleResponse.StatusCode == HttpStatusCode.OK,
            $"Vehicle creation failed with status: {vehicleResponse.StatusCode}");
        var vehicleId = await vehicleResponse.Content.ReadFromJsonAsync<Guid>();

        // Step 3: Create Repair Order
        var orRequest = new
        {
            customerId = customerId,
            vehicleId = vehicleId,
            type = "General",
            visitReason = "Vibration au freinage et bruit suspect à l'avant droit",
            intakeMileage = 45678
        };
        var orResponse = await Client.PostAsJsonAsync("/api/repairorders", orRequest);
        Assert.True(orResponse.StatusCode == HttpStatusCode.Created || orResponse.StatusCode == HttpStatusCode.OK,
            $"Repair order creation failed with status: {orResponse.StatusCode}");
        var orId = await orResponse.Content.ReadFromJsonAsync<Guid>();

        // Step 4: Verify OR Details
        var getResponse = await Client.GetAsync($"/api/repairorders/{orId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var repairOrder = await getResponse.Content.ReadFromJsonAsync<RepairOrderDetailResponse>(JsonOptions);

        Assert.NotNull(repairOrder);
        Assert.Equal("Sophie Laurent", repairOrder!.CustomerName);
        Assert.Contains("Peugeot 208", repairOrder.VehicleInfo);
        Assert.Equal("Vibration au freinage et bruit suspect à l'avant droit", repairOrder.VisitReason);
        Assert.Equal(45678, repairOrder.IntakeMileage);
        Assert.Equal("Draft", repairOrder.Status);
    }

    [Fact]
    public async Task ReceptionFlow_CompanyCustomer_Success()
    {
        // Step 1: Create Company Customer
        var customerRequest = new
        {
            firstName = "Jean",
            lastName = "Martin",
            email = $"contact_{Guid.NewGuid():N}@entreprise.fr",
            phone = "+33144444444",
            type = "Company",
            companyName = "Bosch Car Service Lyon"
        };
        var customerResponse = await Client.PostAsJsonAsync("/api/customers", customerRequest);
        Assert.True(customerResponse.StatusCode == HttpStatusCode.Created || customerResponse.StatusCode == HttpStatusCode.OK,
            $"Company customer creation failed with status: {customerResponse.StatusCode}");
        var customerId = await customerResponse.Content.ReadFromJsonAsync<Guid>();

        // Step 2: Create Vehicle
        var vehicleRequest = new
        {
            vin = $"VIN{Guid.NewGuid():N}"[..17],
            licensePlate = "ENT-001-AA",
            make = "Volkswagen",
            model = "Transporter",
            year = 2023,
            customerId = customerId,
            isInternalFleet = true
        };
        var vehicleResponse = await Client.PostAsJsonAsync("/api/vehicles", vehicleRequest);
        Assert.True(vehicleResponse.StatusCode == HttpStatusCode.Created || vehicleResponse.StatusCode == HttpStatusCode.OK,
            $"Vehicle creation failed with status: {vehicleResponse.StatusCode}");
        var vehicleId = await vehicleResponse.Content.ReadFromJsonAsync<Guid>();

        // Step 3: Create Sinistre OR
        var orRequest = new
        {
            customerId = customerId,
            vehicleId = vehicleId,
            type = "Sinistre",
            visitReason = "Accident - réparation pare-chocs avant et capot",
            intakeMileage = 12500
        };
        var orResponse = await Client.PostAsJsonAsync("/api/repairorders", orRequest);
        Assert.True(orResponse.StatusCode == HttpStatusCode.Created || orResponse.StatusCode == HttpStatusCode.OK,
            $"Sinistre repair order creation failed with status: {orResponse.StatusCode}");
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateTestCustomerAsync(string? email = null)
    {
        var request = new
        {
            firstName = "Test",
            lastName = $"Customer_{Guid.NewGuid():N}"[..8],
            email = email ?? $"test_{Guid.NewGuid():N}@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        var response = await Client.PostAsJsonAsync("/api/customers", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateTestVehicleAsync(Guid customerId, string? licensePlate = null)
    {
        var request = new
        {
            vin = $"VIN{Guid.NewGuid():N}"[..17],
            licensePlate = licensePlate ?? $"XX-{Guid.NewGuid():N}"[..6],
            make = "TestMake",
            model = "TestModel",
            year = 2020,
            customerId = customerId,
            isInternalFleet = false
        };

        var response = await Client.PostAsJsonAsync("/api/vehicles", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateTestRepairOrderAsync(Guid customerId, Guid vehicleId)
    {
        var request = new
        {
            customerId = customerId,
            vehicleId = vehicleId,
            type = "General",
            visitReason = "Test repair order",
            intakeMileage = 50000
        };

        var response = await Client.PostAsJsonAsync("/api/repairorders", request);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    #endregion

    #region Response Types

    private class CustomerDetailResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

    private class CustomersListResponse
    {
        public List<CustomerDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class CustomerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    private class VehicleDetailResponse
    {
        public Guid Id { get; set; }
        public string Vin { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public bool IsInternalFleet { get; set; }
    }

    private class VehiclesListResponse
    {
        public List<VehicleDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class VehicleDto
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
    }

    private class RepairOrderDetailResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid VehicleId { get; set; }
        public string VehicleInfo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string VisitReason { get; set; } = string.Empty;
        public int? IntakeMileage { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

    private class RepairOrdersListResponse
    {
        public List<RepairOrderListDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
    }

    private class RepairOrderListDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    #endregion
}
