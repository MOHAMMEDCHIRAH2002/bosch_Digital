using Tyrex.Application.Messaging;

namespace Tyrex.Application.Commerce.Commands.GenerateEstimate;

public sealed record GenerateEstimateCommand(
    Guid RepairOrderId,
    List<EstimateLineItemCommand> Items) : ICommand<Guid>;

public sealed record EstimateLineItemCommand(
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal TaxRate);
