using Tyrex.SharedKernel.Primitives;

namespace Tyrex.SharedKernel.Exceptions;

public class DomainException : Exception
{
    public DomainException(Error error)
        : base(error.Message)
    {
        Error = error;
    }

    public Error Error { get; }
}
