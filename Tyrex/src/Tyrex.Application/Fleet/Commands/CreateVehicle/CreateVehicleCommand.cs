using Tyrex.Application.Messaging;

namespace Tyrex.Application.Fleet.Commands.CreateVehicle;

public sealed record CreateVehicleCommand(
    string Vin,
    string LicensePlate,
    string Make,
    string Model,
    int Year,
    Guid CustomerId,
    bool IsInternalFleet) : ICommand<Guid>;
