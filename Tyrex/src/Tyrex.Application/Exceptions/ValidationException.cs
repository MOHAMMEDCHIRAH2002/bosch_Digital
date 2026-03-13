using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<Error> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; }
}
