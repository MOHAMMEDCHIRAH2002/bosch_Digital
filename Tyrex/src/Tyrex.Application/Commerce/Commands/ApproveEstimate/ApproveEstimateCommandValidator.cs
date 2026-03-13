using FluentValidation;

namespace Tyrex.Application.Commerce.Commands.ApproveEstimate;

public class ApproveEstimateCommandValidator : AbstractValidator<ApproveEstimateCommand>
{
    public ApproveEstimateCommandValidator()
    {
        RuleFor(x => x.EstimateId).NotEmpty();
        RuleFor(x => x.ClientApprovalProofUrl).NotEmpty();
    }
}
