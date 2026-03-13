using Tyrex.Application.Messaging;

namespace Tyrex.Application.Commerce.Commands.SendEstimateEmail;

public sealed record SendEstimateEmailCommand(
    Guid EstimateId,
    string ToEmail) : ICommand;
