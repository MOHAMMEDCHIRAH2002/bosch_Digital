using MediatR;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Messaging;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
