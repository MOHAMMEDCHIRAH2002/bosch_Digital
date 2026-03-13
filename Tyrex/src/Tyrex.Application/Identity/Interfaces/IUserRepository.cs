using Tyrex.Domain.Identity;

namespace Tyrex.Application.Identity.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    void Add(User user);
}
