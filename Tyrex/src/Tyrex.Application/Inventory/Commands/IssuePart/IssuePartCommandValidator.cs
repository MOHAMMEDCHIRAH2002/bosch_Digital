using FluentValidation;

namespace Tyrex.Application.Inventory.Commands.IssuePart;

public class IssuePartCommandValidator : AbstractValidator<IssuePartCommand>
{
    public IssuePartCommandValidator()
    {
        RuleFor(x => x.PartNumber).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.RepairOrderId).NotEmpty();
    }
}
