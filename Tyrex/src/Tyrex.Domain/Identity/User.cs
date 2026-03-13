using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Identity;

public sealed class User : AggregateRoot
{
    private User(Guid id, string email, string passwordHash, string firstName, string lastName)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        Role = Identity.Role.Technician; // Default
    }

    private User()
    {
    }

    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    
    public Role Role { get; private set; }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        return new User(Guid.NewGuid(), email, passwordHash, firstName, lastName);
    }

    public void AssignRole(Role role)
    {
        Role = role;
    }
}
