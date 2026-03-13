using MediatR;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
