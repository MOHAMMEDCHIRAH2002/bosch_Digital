using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.CRM.Queries.GetCustomers;

internal sealed class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, CustomersListResponse>
{
    private readonly IApplicationDbContext _context;

    public GetCustomersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CustomersListResponse>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Set<Customer>().AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(c =>
                EF.Functions.Like(c.FirstName.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(c.LastName.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(c.Email.ToLower(), $"%{searchLower}%") ||
                EF.Functions.Like(c.Phone.ToLower(), $"%{searchLower}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedOnUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CustomerDto(
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                c.Type.ToString(),
                c.CompanyName,
                c.CreatedOnUtc))
            .ToListAsync(cancellationToken);

        return new CustomersListResponse(items, totalCount, request.Page, request.PageSize);
    }
}
