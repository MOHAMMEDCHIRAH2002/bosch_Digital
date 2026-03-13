using Tyrex.Application.Messaging;

namespace Tyrex.Application.Workshop.Commands.StartRepair;

public sealed record StartRepairCommand(
    Guid RepairOrderId,
    Guid TechnicianId) : ICommand<Guid>;
