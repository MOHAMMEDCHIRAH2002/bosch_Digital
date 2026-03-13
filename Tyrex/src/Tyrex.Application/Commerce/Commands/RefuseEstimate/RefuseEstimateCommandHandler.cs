using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Commerce.Commands.RefuseEstimate;

internal sealed class RefuseEstimateCommandHandler : ICommandHandler<RefuseEstimateCommand>
{
    private readonly IEstimateRepository _estimateRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RefuseEstimateCommandHandler(
        IEstimateRepository estimateRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _estimateRepository = estimateRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RefuseEstimateCommand request, CancellationToken cancellationToken)
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

        estimate.Refuse();
        repairOrder.TransitionTo(RepairOrderStatus.EstimateRefused);

        // TODO MVP: Depending on refusal reason, order may be closed (ClosedUnrepaired) or paused. MVP simply changes status.
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

