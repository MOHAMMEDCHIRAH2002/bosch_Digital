using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Commerce.Commands.ApproveEstimate;

internal sealed class ApproveEstimateCommandHandler : ICommandHandler<ApproveEstimateCommand>
{
    private readonly IEstimateRepository _estimateRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveEstimateCommandHandler(
        IEstimateRepository estimateRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _estimateRepository = estimateRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveEstimateCommand request, CancellationToken cancellationToken)
    {
        var estimate = await _estimateRepository.GetByIdAsync(request.EstimateId, cancellationToken);
        if (estimate is null)
        {
            return Result.Failure(Error.NotFound("Estimate.NotFound", "The estimate was not found."));
        }

        var repairOrder = await _repairOrderRepository.GetByIdAsync(estimate.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure(Error.NotFound("RepairOrder.NotFound", "Associated repair order not found."));
        }

        estimate.Approve(request.ClientApprovalProofUrl);
        repairOrder.TransitionTo(RepairOrderStatus.EstimateApproved);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

