using FluentValidation;

namespace Tyrex.Application.Billing.Commands.GenerateInvoice;

public class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty().GreaterThanOrEqualTo(DateTime.UtcNow.Date);
    }
}
