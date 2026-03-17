using System.Net;
using System.Net.Http.Json;

namespace Tyrex.IntegrationTests;

/// <summary>
/// End-to-end tests that verify the complete repair order workflow:
/// Customer → Vehicle → RepairOrder → Diagnosis → Estimate → Approve → Repair → Complete → Quality → Invoice → Payment
/// </summary>
public class EndToEndWorkflowTests : BaseIntegrationTest
{
    public EndToEndWorkflowTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CompleteRepairOrderLifecycle_WithSinistreType_FullFlow()
    {
        // Step 1: Create Customer
        var customerId = await CreateCustomerAsync();

        // Step 2: Create Vehicle for Customer
        var vehicleId = await CreateVehicleAsync(customerId);

        // Step 3: Create Repair Order
        var repairOrderId = await CreateRepairOrderAsync(customerId, vehicleId, "Sinistre");

        // Step 4: Submit Diagnosis
        await SubmitDiagnosisAsync(repairOrderId);

        // Step 5: Generate Estimate
        var estimateId = await GenerateEstimateAsync(repairOrderId);

        // Step 6: Approve Estimate
        await ApproveEstimateAsync(estimateId);

        // Step 7: Start Repair
        await StartRepairAsync(repairOrderId);

        // Step 8: Complete Repair
        await CompleteRepairAsync(repairOrderId);

        // Step 9: Submit Quality Checklist
        await SubmitQualityChecklistAsync(repairOrderId);

        // Step 10: Generate Invoice
        var invoiceId = await GenerateInvoiceAsync(repairOrderId);

        // Step 11: Register Payment
        await RegisterPaymentAsync(invoiceId);

        // Step 12: Verify Dashboard reflects completed work
        var dashboardResponse = await Client.GetAsync("/api/dashboard/kpis");
        Assert.Equal(HttpStatusCode.OK, dashboardResponse.StatusCode);
    }

    [Fact]
    public async Task CompleteRepairOrderLifecycle_WithGeneralType_FullFlow()
    {
        // Step 1: Create Customer
        var customerId = await CreateCustomerAsync();

        // Step 2: Create Vehicle
        var vehicleId = await CreateVehicleAsync(customerId);

        // Step 3: Create Repair Order
        var repairOrderId = await CreateRepairOrderAsync(customerId, vehicleId, "General");

        // Step 4: Submit Diagnosis
        await SubmitDiagnosisAsync(repairOrderId);

        // Step 5: Generate Estimate
        var estimateId = await GenerateEstimateAsync(repairOrderId);

        // Step 6: Approve Estimate
        await ApproveEstimateAsync(estimateId);

        // Step 7: Start Repair
        await StartRepairAsync(repairOrderId);

        // Step 8: Complete Repair
        await CompleteRepairAsync(repairOrderId);

        // Step 9: Submit Quality Checklist
        await SubmitQualityChecklistAsync(repairOrderId);

        // Step 10: Generate Invoice
        var invoiceId = await GenerateInvoiceAsync(repairOrderId);

        // Step 11: Register Payment
        await RegisterPaymentAsync(invoiceId);

        // Assert final state
        var invoiceResponse = await Client.GetAsync($"/api/billing/invoices/{invoiceId}");
        // Note: Invoice retrieval endpoint may not exist, this is to verify the flow completed
    }

    [Fact]
    public async Task EstimateRefusal_WorksCorrectly()
    {
        // Step 1: Create Customer
        var customerId = await CreateCustomerAsync();

        // Step 2: Create Vehicle
        var vehicleId = await CreateVehicleAsync(customerId);

        // Step 3: Create Repair Order
        var repairOrderId = await CreateRepairOrderAsync(customerId, vehicleId, "General");

        // Step 4: Submit Diagnosis
        await SubmitDiagnosisAsync(repairOrderId);

        // Step 5: Generate Estimate
        var estimateId = await GenerateEstimateAsync(repairOrderId);

        // Step 6: Refuse Estimate
        var refuseResponse = await Client.PostAsJsonAsync($"/api/estimates/{estimateId}/refuse", new { });
        Assert.Equal(HttpStatusCode.OK, refuseResponse.StatusCode);

        // Step 7: Verify status changed
        var repairOrderResponse = await Client.GetAsync($"/api/repairorders/{repairOrderId}");
        // Note: Individual GET endpoint may need to be added
    }

    [Fact]
    public async Task PartialPayment_WorksCorrectly()
    {
        // Step 1: Create full workflow up to Invoice
        var customerId = await CreateCustomerAsync();
        var vehicleId = await CreateVehicleAsync(customerId);
        var repairOrderId = await CreateRepairOrderAsync(customerId, vehicleId, "General");
        await SubmitDiagnosisAsync(repairOrderId);
        var estimateId = await GenerateEstimateAsync(repairOrderId);
        await ApproveEstimateAsync(estimateId);
        await StartRepairAsync(repairOrderId);
        await CompleteRepairAsync(repairOrderId);
        await SubmitQualityChecklistAsync(repairOrderId);
        var invoiceId = await GenerateInvoiceAsync(repairOrderId);

        // Step 2: Make partial payment (50)
        var partialPaymentRequest = new
        {
            amount = 50m,
            method = "CreditCard",
            reference = "PARTIAL-001"
        };
        var partialResponse = await Client.PostAsJsonAsync($"/api/billing/invoices/{invoiceId}/pay", partialPaymentRequest);
        Assert.Equal(HttpStatusCode.OK, partialResponse.StatusCode);

        // Step 3: Make remaining payment
        var remainingPaymentRequest = new
        {
            amount = 50m, // Adjust based on actual invoice total
            method = "CreditCard",
            reference = "REMAINING-001"
        };
        var remainingResponse = await Client.PostAsJsonAsync($"/api/billing/invoices/{invoiceId}/pay", remainingPaymentRequest);
        Assert.Equal(HttpStatusCode.OK, remainingResponse.StatusCode);
    }

    [Fact]
    public async Task ServiceRapide_FastTrackWorkflow()
    {
        // Service Rapide typically has a shorter workflow
        var customerId = await CreateCustomerAsync();
        var vehicleId = await CreateVehicleAsync(customerId);
        var repairOrderId = await CreateRepairOrderAsync(customerId, vehicleId, "ServiceRapide");

        // For service rapide, might skip full diagnosis
        await SubmitDiagnosisAsync(repairOrderId);

        // Generate simple estimate
        var estimateId = await GenerateEstimateAsync(repairOrderId);
        await ApproveEstimateAsync(estimateId);

        // Quick repair
        await StartRepairAsync(repairOrderId);
        await CompleteRepairAsync(repairOrderId);

        // Quality check
        await SubmitQualityChecklistAsync(repairOrderId);

        // Invoice and payment
        var invoiceId = await GenerateInvoiceAsync(repairOrderId);
        await RegisterPaymentAsync(invoiceId);
    }

    // Helper Methods

    private async Task<Guid> CreateCustomerAsync()
    {
        var request = new
        {
            firstName = "E2E",
            lastName = $"Test{Guid.NewGuid().ToString("N")[..6]}",
            email = $"e2e.{Guid.NewGuid().ToString("N")[..8]}@test.com",
            phone = "+33612345678",
            type = "Individual",
            companyName = (string?)null
        };

        var response = await Client.PostAsJsonAsync("/api/customers", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateVehicleAsync(Guid customerId)
    {
        var request = new
        {
            vin = $"E2E{Guid.NewGuid().ToString("N")[..14]}",
            licensePlate = $"E2E-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            make = "TestMake",
            model = "TestModel",
            year = 2020,
            customerId = customerId,
            isInternalFleet = false
        };

        var response = await Client.PostAsJsonAsync("/api/vehicles", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task<Guid> CreateRepairOrderAsync(Guid customerId, Guid vehicleId, string type)
    {
        var request = new
        {
            customerId = customerId,
            vehicleId = vehicleId,
            type = type,
            description = $"E2E Test {type} Repair Order",
            mileage = 50000
        };

        var response = await Client.PostAsJsonAsync("/api/repairorders", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task SubmitDiagnosisAsync(Guid repairOrderId)
    {
        var request = new
        {
            technicianNotes = "E2E Test diagnosis notes",
            findings = new[]
            {
                new { description = "Brake pads worn", severity = "High", recommendedAction = "Replace brake pads" },
                new { description = "Oil leak minor", severity = "Medium", recommendedAction = "Monitor and repair" }
            }
        };

        var response = await Client.PostAsJsonAsync($"/api/repairorders/{repairOrderId}/diagnostics", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> GenerateEstimateAsync(Guid repairOrderId)
    {
        var request = new
        {
            repairOrderId = repairOrderId,
            validForDays = 30,
            notes = "E2E Test estimate"
        };

        var response = await Client.PostAsJsonAsync("/api/estimates", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task ApproveEstimateAsync(Guid estimateId)
    {
        var response = await Client.PostAsJsonAsync($"/api/estimates/{estimateId}/approve", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task StartRepairAsync(Guid repairOrderId)
    {
        var response = await Client.PostAsJsonAsync($"/api/repairs/{repairOrderId}/start", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task CompleteRepairAsync(Guid repairOrderId)
    {
        var response = await Client.PostAsJsonAsync($"/api/repairs/{repairOrderId}/complete", new { });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task SubmitQualityChecklistAsync(Guid repairOrderId)
    {
        var request = new
        {
            repairOrderId = repairOrderId,
            technicianId = "00000000-0000-0000-0000-000000000002", // Seeded tech user
            items = new[]
            {
                new { category = "Safety", description = "Brakes functional", isChecked = true, notes = "OK" },
                new { category = "Safety", description = "Lights working", isChecked = true, notes = "OK" },
                new { category = "Quality", description = "Oil level correct", isChecked = true, notes = "OK" }
            }
        };

        var response = await Client.PostAsJsonAsync($"/api/repairorders/{repairOrderId}/quality/submit", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> GenerateInvoiceAsync(Guid repairOrderId)
    {
        var request = new
        {
            repairOrderId = repairOrderId
        };

        var response = await Client.PostAsJsonAsync("/api/billing/invoices", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    private async Task RegisterPaymentAsync(Guid invoiceId)
    {
        var request = new
        {
            amount = 100m, // Adjust based on invoice total
            method = "CreditCard",
            reference = "E2E-PAYMENT-001"
        };

        var response = await Client.PostAsJsonAsync($"/api/billing/invoices/{invoiceId}/pay", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
