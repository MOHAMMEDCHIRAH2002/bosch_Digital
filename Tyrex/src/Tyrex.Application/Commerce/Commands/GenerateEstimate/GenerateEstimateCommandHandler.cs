using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Commerce;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Commerce.Commands.GenerateEstimate;

internal sealed class GenerateEstimateCommandHandler : ICommandHandler<GenerateEstimateCommand, Guid>
{
    private readonly IEstimateRepository _estimateRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateEstimateCommandHandler(
        IEstimateRepository estimateRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _estimateRepository = estimateRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(GenerateEstimateCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure<Guid>(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
        }

        // Verify status allows estimation
        if (repairOrder.Status < RepairOrderStatus.EstimateReady || repairOrder.Status >= RepairOrderStatus.EstimateApproved)
        {
            return Result.Failure<Guid>(Error.Validation("RepairOrder.InvalidState", "Repair order is not in a valid state for estimation."));
        }

        var existingEstimate = await _estimateRepository.GetActiveByRepairOrderIdAsync(request.RepairOrderId, cancellationToken);
        
        Estimate estimate;
        if (existingEstimate is null)
        {
            estimate = Estimate.CreateInitial(request.RepairOrderId);
        }
        else
        {
            estimate = existingEstimate.CreateNextVersion();
            // In a real app we might mark the previous one as Superseded. MVP implies active replacement.
        }

        foreach (var item in request.Items)
        {
            estimate.AddLineItem(item.Description, item.Quantity, item.UnitPrice, item.TaxRate);
        }

        _estimateRepository.Add(estimate);
        
        repairOrder.TransitionTo(RepairOrderStatus.AwaitingCustomerApproval);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return estimate.Id;
    }
}

