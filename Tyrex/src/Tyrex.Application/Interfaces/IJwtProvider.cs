namespace Tyrex.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(Guid userId, string email, IList<string> roles);
}
