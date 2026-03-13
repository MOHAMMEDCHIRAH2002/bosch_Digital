using FluentValidation;

namespace Tyrex.Application.Fleet.Commands.CreateVehicle;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(v => v.LicensePlate).NotEmpty();
        RuleFor(v => v.Make).NotEmpty();
        RuleFor(v => v.Model).NotEmpty();
        RuleFor(v => v.Year).GreaterThan(1900).LessThanOrEqualTo(DateTime.UtcNow.Year + 1);
        RuleFor(v => v.CustomerId).NotEmpty();
    }
}
