using System.Net;
using System.Net.Http.Json;

namespace Tyrex.IntegrationTests;

public class DashboardEndpointsTests : BaseIntegrationTest
{
    public DashboardEndpointsTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetKpis_ReturnsDashboardData()
    {
        // Act
        var response = await Client.GetAsync("/api/dashboard/kpis");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<DashboardKpisResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.True(result.ActiveRepairOrders >= 0);
        Assert.True(result.PendingEstimates >= 0);
        Assert.True(result.VehiclesReadyForPickup >= 0);
        Assert.True(result.TodayRevenue >= 0);
    }

    [Fact]
    public async Task GetActiveRepairOrders_ReturnsList()
    {
        // Act
        var response = await Client.GetAsync("/api/dashboard/active-orders");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<List<ActiveRepairOrderResponse>>(JsonOptions);
        Assert.NotNull(result);
    }

    private class DashboardKpisResponse
    {
        public int ActiveRepairOrders { get; set; }
        public int PendingEstimates { get; set; }
        public int VehiclesReadyForPickup { get; set; }
        public decimal TodayRevenue { get; set; }
    }

    private class ActiveRepairOrderResponse
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string VehicleInfo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
