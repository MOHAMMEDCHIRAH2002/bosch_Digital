using Tyrex.Application.Identity.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Identity.Commands.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(new Error("User.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized));
        }

        // Extremely simple password verification for MVP. 
        if (user.PasswordHash != request.Password) 
        {
            return Result.Failure<LoginResponse>(new Error("User.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized));
        }

        string token = _jwtProvider.Generate(user.Id, user.Email, new[] { user.Role.ToString() });

        return new LoginResponse(
            token,
            "mock-refresh-token",
            user.Id,
            user.Email,
            user.Role.ToString());
    }
}
