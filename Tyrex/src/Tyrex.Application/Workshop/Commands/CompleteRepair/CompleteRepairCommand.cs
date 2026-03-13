using Tyrex.Application.Messaging;

namespace Tyrex.Application.Workshop.Commands.CompleteRepair;

public sealed record CompleteRepairCommand(
    Guid RepairOrderId,
    Guid TechnicianId) : ICommand;
