namespace Tyrex.SharedKernel.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
