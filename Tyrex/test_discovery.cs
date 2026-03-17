// Add this test temporarily to Module01ReceptionTests.cs
[Fact]
public async Task Debug_ControllerDiscovery()
{
    // Try to access swagger endpoint to see if API is working
    var swaggerResponse = await Client.GetAsync("/swagger/v1/swagger.json");
    Console.WriteLine($"Swagger: {swaggerResponse.StatusCode}");
    
    // Try accessing a known working endpoint (customers)
    var customerResponse = await Client.GetAsync("/api/customers");
    Console.WriteLine($"Customers GET: {customerResponse.StatusCode}");
    
    // Try accessing repair orders with GET (should work if controller exists)
    var repairOrdersResponse = await Client.GetAsync("/api/repair-orders");
    Console.WriteLine($"RepairOrders GET: {repairOrdersResponse.StatusCode}");
}
