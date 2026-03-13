using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Fleet;

public sealed class Vehicle : AggregateRoot, IAuditableEntity
{
    private Vehicle(Guid id, string vin, string licensePlate, string make, string model, int year, Guid customerId, bool isInternalFleet)
        : base(id)
    {
        Vin = vin;
        LicensePlate = licensePlate;
        Make = make;
        Model = model;
        Year = year;
        CustomerId = customerId;
        IsInternalFleet = isInternalFleet;
    }

    private Vehicle()
    {
    }

    public string Vin { get; private set; } = string.Empty;
    public string LicensePlate { get; private set; } = string.Empty;
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public Guid CustomerId { get; private set; }
    
    // Internal company vehicles handling rule
    public bool IsInternalFleet { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static Vehicle Create(string vin, string licensePlate, string make, string model, int year, Guid customerId, bool isInternalFleet = false)
    {
        // TODO: Validate VIN/License Plate format using value objects in real domain
        return new Vehicle(Guid.NewGuid(), vin, licensePlate, make, model, year, customerId, isInternalFleet);
    }
}
