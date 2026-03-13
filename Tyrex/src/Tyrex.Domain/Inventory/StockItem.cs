using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Inventory;

public sealed class StockItem : AggregateRoot, IAuditableEntity
{
    private StockItem(Guid id, string partNumber, string description, string location, int initialQuantity)
        : base(id)
    {
        PartNumber = partNumber;
        Description = description;
        Location = location;
        QuantityOnHand = initialQuantity;
    }

    private StockItem()
    {
    }

    public string PartNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public int QuantityOnHand { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static StockItem Create(string partNumber, string description, string location, int initialQuantity = 0)
    {
        return new StockItem(Guid.NewGuid(), partNumber, description, location, initialQuantity);
    }

    public Result AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(Error.Validation("StockItem.InvalidQuantity", "Quantity must be greater than zero."));
        }

        QuantityOnHand += quantity;
        return Result.Success();
    }

    public Result IssueStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result.Failure(Error.Validation("StockItem.InvalidQuantity", "Quantity must be greater than zero."));
        }

        if (QuantityOnHand < quantity)
        {
            return Result.Failure(Error.Failure("StockItem.InsufficientStock", "Not enough stock available."));
        }

        QuantityOnHand -= quantity;
        return Result.Success();
    }
}

