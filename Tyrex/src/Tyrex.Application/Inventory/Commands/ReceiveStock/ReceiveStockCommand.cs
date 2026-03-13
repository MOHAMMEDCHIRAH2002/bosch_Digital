using Tyrex.Application.Messaging;

namespace Tyrex.Application.Inventory.Commands.ReceiveStock;

public sealed record ReceiveStockCommand(
    string PartNumber,
    string Description,
    string Location,
    int Quantity) : ICommand<Guid>;
