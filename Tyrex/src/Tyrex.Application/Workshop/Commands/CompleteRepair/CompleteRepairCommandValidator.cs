using FluentValidation;

namespace Tyrex.Application.Workshop.Commands.CompleteRepair;

public class CompleteRepairCommandValidator : AbstractValidator<CompleteRepairCommand>
{
    public CompleteRepairCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.TechnicianId).NotEmpty();
    }
}
