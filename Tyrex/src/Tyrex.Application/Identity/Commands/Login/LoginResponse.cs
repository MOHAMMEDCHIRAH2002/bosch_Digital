namespace Tyrex.Application.Identity.Commands.Login;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string Email,
    string Role);
