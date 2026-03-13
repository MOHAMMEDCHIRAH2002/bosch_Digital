using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;

namespace Tyrex.Application.CRM.Commands.CreateCustomer;

public sealed record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    CustomerType Type,
    string? CompanyName) : ICommand<Guid>;
