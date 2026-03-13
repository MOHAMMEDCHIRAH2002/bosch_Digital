using Tyrex.Application.Messaging;

namespace Tyrex.Application.Billing.Commands.RegisterPayment;

public sealed record RegisterPaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    Domain.Billing.PaymentMethod Method,
    string? ReferenceInfo) : ICommand;
