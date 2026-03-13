using FluentValidation;

namespace Tyrex.Application.Workshop.Commands.CreateRepairOrder;

public class CreateRepairOrderCommandValidator : AbstractValidator<CreateRepairOrderCommand>
{
    public CreateRepairOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.VisitReason).NotEmpty();
    }
}
