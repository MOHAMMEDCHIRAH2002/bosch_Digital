using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.CRM.Queries.GetCustomerById;

internal sealed class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDetailResponse>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomerDetailResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Set<Customer>()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer is null)
        {
            return Result.Failure<CustomerDetailResponse>(Error.NotFound("Customer.NotFound", "Customer not found."));
        }

        var response = new CustomerDetailResponse(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Phone,
            customer.Type.ToString(),
            customer.CompanyName,
            customer.CreatedOnUtc);

        return Result.Success(response);
    }
}
