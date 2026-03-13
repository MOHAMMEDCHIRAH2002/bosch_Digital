using FluentValidation;

namespace Tyrex.Application.Inventory.Commands.ReceiveStock;

public class ReceiveStockCommandValidator : AbstractValidator<ReceiveStockCommand>
{
    public ReceiveStockCommandValidator()
    {
        RuleFor(x => x.PartNumber).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
