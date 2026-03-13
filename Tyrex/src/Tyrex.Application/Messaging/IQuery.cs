using MediatR;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
