using FluentValidation;
using Tyrex.Domain.CRM;

namespace Tyrex.Application.CRM.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Phone).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
        
        RuleFor(c => c.CompanyName)
            .NotEmpty()
            .When(c => c.Type == CustomerType.Company)
            .WithMessage("Company Name is required for Company customers");
    }
}
