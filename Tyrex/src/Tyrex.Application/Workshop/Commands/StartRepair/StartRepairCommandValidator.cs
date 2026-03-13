using FluentValidation;

namespace Tyrex.Application.Workshop.Commands.StartRepair;

public class StartRepairCommandValidator : AbstractValidator<StartRepairCommand>
{
    public StartRepairCommandValidator()
    {
        RuleFor(x => x.RepairOrderId).NotEmpty();
        RuleFor(x => x.TechnicianId).NotEmpty();
    }
}
