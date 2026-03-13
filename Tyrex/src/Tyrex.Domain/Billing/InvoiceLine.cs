using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Billing;

public sealed class InvoiceLine : Entity
{
    internal InvoiceLine(Guid id, string description, int quantity, decimal unitPrice, decimal taxRate)
        : base(id)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }

    private InvoiceLine() { }

    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    
    public decimal TotalExcludingTax => Quantity * UnitPrice;
    public decimal TotalIncludingTax => TotalExcludingTax * (1 + TaxRate);
}
