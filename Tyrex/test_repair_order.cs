using var client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5101");

// Login first
var loginRequest = new { email = "admin@tyrex.com", password = "admin123" };
var loginJson = System.Text.Json.JsonSerializer.Serialize(loginRequest);
var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");
var loginResponse = await client.PostAsync("/api/auth/login", loginContent);
Console.WriteLine($"Login: {loginResponse.StatusCode}");
var loginResult = await loginResponse.Content.ReadAsStringAsync();
Console.WriteLine(loginResult);

// Get token and set auth header
using var doc = System.Text.Json.JsonDocument.Parse(loginResult);
var token = doc.RootElement.GetProperty("accessToken").GetString();
client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

// Create customer
var customerRequest = new { firstName = "Test", lastName = "Customer", email = $"test{Guid.NewGuid()}@test.com", phone = "+33612345678", type = "Individual", companyName = (string?)null };
var customerJson = System.Text.Json.JsonSerializer.Serialize(customerRequest);
var customerContent = new StringContent(customerJson, System.Text.Encoding.UTF8, "application/json");
var customerResponse = await client.PostAsync("/api/customers", customerContent);
Console.WriteLine($"Create customer: {customerResponse.StatusCode}");
var customerResult = await customerResponse.Content.ReadAsStringAsync();
Console.WriteLine(customerResult);

// Create vehicle
var customerId = Guid.Parse(customerResult.Trim('"'));
var vehicleRequest = new { vin = $"VIN{Guid.NewGuid():N}"[..17], licensePlate = "XX-123-XX", make = "Test", model = "Test", year = 2020, customerId = customerId, isInternalFleet = false };
var vehicleJson = System.Text.Json.JsonSerializer.Serialize(vehicleRequest);
var vehicleContent = new StringContent(vehicleJson, System.Text.Encoding.UTF8, "application/json");
var vehicleResponse = await client.PostAsync("/api/vehicles", vehicleContent);
Console.WriteLine($"Create vehicle: {vehicleResponse.StatusCode}");
var vehicleResult = await vehicleResponse.Content.ReadAsStringAsync();
Console.WriteLine(vehicleResult);

// Create repair order
var vehicleId = Guid.Parse(vehicleResult.Trim('"'));
var orRequest = new { customerId = customerId, vehicleId = vehicleId, type = "General", visitReason = "Test", intakeMileage = 50000 };
var orJson = System.Text.Json.JsonSerializer.Serialize(orRequest);
Console.WriteLine($"Request JSON: {orJson}");
var orContent = new StringContent(orJson, System.Text.Encoding.UTF8, "application/json");
var orResponse = await client.PostAsync("/api/repair-orders", orContent);
Console.WriteLine($"Create repair order: {orResponse.StatusCode}");
var orResult = await orResponse.Content.ReadAsStringAsync();
Console.WriteLine(orResult);
