using Microsoft.EntityFrameworkCore;
using Tyrex.Application.Identity.Interfaces;
using Tyrex.Domain.Identity;

namespace Tyrex.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == id, ct);

    public void Add(User user) => _context.Set<User>().Add(user);
}
