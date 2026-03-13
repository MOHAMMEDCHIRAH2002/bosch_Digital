using Tyrex.Application.Interfaces;
using Tyrex.Application.Inventory.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Inventory.Commands.IssuePart;

internal sealed class IssuePartCommandHandler : ICommandHandler<IssuePartCommand>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IssuePartCommandHandler(
        IStockItemRepository stockItemRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(IssuePartCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure(Error.NotFound("RepairOrder.NotFound", "Repair order not found."));
        }

        var stockItem = await _stockItemRepository.GetByPartNumberAsync(request.PartNumber, cancellationToken);
        if (stockItem is null)
        {
            return Result.Failure(Error.NotFound("StockItem.NotFound", "Stock item not found."));
        }

        var result = stockItem.IssueStock(request.Quantity);
        if (result.IsFailure)
        {
            return result;
        }

        // Keep simplified for MVP: We assume the part is issued, maybe update RO status if it was waiting for parts
        if (repairOrder.Status == Domain.Workshop.RepairOrderStatus.WaitingPart)
        {
            repairOrder.TransitionTo(Domain.Workshop.RepairOrderStatus.InRepair);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

