using Tyrex.Application.Messaging;

namespace Tyrex.Application.CRM.Queries.GetCustomerById;

public sealed record GetCustomerByIdQuery(Guid Id) : IQuery<CustomerDetailResponse>;

public sealed record CustomerDetailResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Type,
    string? CompanyName,
    DateTime CreatedOnUtc
);
