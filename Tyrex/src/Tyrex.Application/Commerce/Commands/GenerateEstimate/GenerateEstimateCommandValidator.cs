using FluentValidation;

namespace Tyrex.Application.Commerce.Commands.GenerateEstimate;

public class GenerateEstimateCommandValidator : AbstractValidator<GenerateEstimateCommand>
{
    public GenerateEstimateCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new EstimateLineItemCommandValidator());
    }
}

public class EstimateLineItemCommandValidator : AbstractValidator<EstimateLineItemCommand>
{
    public EstimateLineItemCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0);
    }
}
