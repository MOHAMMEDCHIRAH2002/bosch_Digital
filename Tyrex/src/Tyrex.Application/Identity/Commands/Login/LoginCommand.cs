using Tyrex.Application.Messaging;

namespace Tyrex.Application.Identity.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;
