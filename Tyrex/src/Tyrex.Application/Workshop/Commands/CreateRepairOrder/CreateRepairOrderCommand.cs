using Tyrex.Application.Messaging;
using Tyrex.Domain.Workshop;

namespace Tyrex.Application.Workshop.Commands.CreateRepairOrder;

public sealed record CreateRepairOrderCommand(
    Guid CustomerId,
    Guid VehicleId,
    RepairOrderType Type,
    string VisitReason,
    int? IntakeMileage = null) : ICommand<Guid>;
