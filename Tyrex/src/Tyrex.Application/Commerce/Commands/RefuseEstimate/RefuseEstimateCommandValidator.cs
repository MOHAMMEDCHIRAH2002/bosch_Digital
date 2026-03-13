using FluentValidation;

namespace Tyrex.Application.Commerce.Commands.RefuseEstimate;

public class RefuseEstimateCommandValidator : AbstractValidator<RefuseEstimateCommand>
{
    public RefuseEstimateCommandValidator()
    {
        RuleFor(x => x.EstimateId).NotEmpty();
    }
}
