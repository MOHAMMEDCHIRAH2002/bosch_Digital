using Tyrex.Application.Messaging;

namespace Tyrex.Application.Commerce.Commands.ApproveEstimate;

public sealed record ApproveEstimateCommand(
    Guid EstimateId,
    string ClientApprovalProofUrl) : ICommand;
