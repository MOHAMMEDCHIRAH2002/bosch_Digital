using Tyrex.Application.Interfaces;
using Tyrex.Application.Inventory.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Inventory;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Inventory.Commands.ReceiveStock;

internal sealed class ReceiveStockCommandHandler : ICommandHandler<ReceiveStockCommand, Guid>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceiveStockCommandHandler(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ReceiveStockCommand request, CancellationToken cancellationToken)
    {
        var existingItem = await _stockItemRepository.GetByPartNumberAsync(request.PartNumber, cancellationToken);

        if (existingItem is not null)
        {
            var result = existingItem.AddStock(request.Quantity);
            if (result.IsFailure) return Result.Failure<Guid>(result.Error);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return existingItem.Id;
        }

        var newItem = StockItem.Create(request.PartNumber, request.Description, request.Location, request.Quantity);
        _stockItemRepository.Add(newItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newItem.Id;
    }
}
