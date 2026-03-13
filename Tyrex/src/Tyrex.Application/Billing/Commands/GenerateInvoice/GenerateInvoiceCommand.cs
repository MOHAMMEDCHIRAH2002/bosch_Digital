using Tyrex.Application.Messaging;

namespace Tyrex.Application.Billing.Commands.GenerateInvoice;

public sealed record GenerateInvoiceCommand(
    Guid RepairOrderId,
    DateTime DueDate) : ICommand<Guid>;
