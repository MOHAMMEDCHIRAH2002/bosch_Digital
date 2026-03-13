using Tyrex.Application.Messaging;

namespace Tyrex.Application.Inventory.Commands.IssuePart;

public sealed record IssuePartCommand(
    string PartNumber,
    int Quantity,
    Guid RepairOrderId) : ICommand;
