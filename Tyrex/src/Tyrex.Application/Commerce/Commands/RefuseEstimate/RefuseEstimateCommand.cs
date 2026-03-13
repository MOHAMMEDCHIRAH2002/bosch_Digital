using Tyrex.Application.Messaging;

namespace Tyrex.Application.Commerce.Commands.RefuseEstimate;

public sealed record RefuseEstimateCommand(
    Guid EstimateId) : ICommand;
