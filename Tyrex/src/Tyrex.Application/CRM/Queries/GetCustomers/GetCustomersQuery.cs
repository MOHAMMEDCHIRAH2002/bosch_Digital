using Tyrex.Application.Messaging;

namespace Tyrex.Application.CRM.Queries.GetCustomers;

public sealed record GetCustomersQuery(string? SearchTerm = null, int Page = 1, int PageSize = 20) : IQuery<CustomersListResponse>;

public sealed record CustomersListResponse(
    List<CustomerDto> Items,
    int TotalCount,
    int Page,
    int PageSize
);

public sealed record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string Type,
    string? CompanyName,
    DateTime CreatedOnUtc
);
