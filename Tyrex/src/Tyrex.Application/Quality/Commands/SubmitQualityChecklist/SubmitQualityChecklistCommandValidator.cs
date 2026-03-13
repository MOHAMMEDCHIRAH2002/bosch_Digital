using FluentValidation;

namespace Tyrex.Application.Quality.Commands.SubmitQualityChecklist;

public class SubmitQualityChecklistCommandValidator : AbstractValidator<SubmitQualityChecklistCommand>
{
    public SubmitQualityChecklistCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.InspectorId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).SetValidator(new ChecklistItemResultValidator());
    }
}

public class ChecklistItemResultValidator : AbstractValidator<ChecklistItemResult>
{
    public ChecklistItemResultValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Status).IsInEnum().NotEqual(Domain.Quality.QualityCheckItemStatus.Pending); // Must be pass/fail/NA
    }
}
