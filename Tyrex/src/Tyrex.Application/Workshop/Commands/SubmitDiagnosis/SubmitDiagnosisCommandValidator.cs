using FluentValidation;

namespace Tyrex.Application.Workshop.Commands.SubmitDiagnosis;

public class SubmitDiagnosisCommandValidator : AbstractValidator<SubmitDiagnosisCommand>
{
    public SubmitDiagnosisCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.TechnicianId).NotEmpty();
        RuleFor(x => x.Notes).NotEmpty();
        RuleFor(x => x.MediaUrls).NotNull();
    }
}
