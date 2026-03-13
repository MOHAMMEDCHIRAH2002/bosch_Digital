using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Billing;

public sealed class Invoice : AggregateRoot, IAuditableEntity
{
    private readonly List<InvoiceLine> _lines = new();
    private readonly List<Payment> _payments = new();

    private Invoice(Guid id, string invoiceNumber, Guid repairOrderId, DateTime dueDate)
        : base(id)
    {
        InvoiceNumber = invoiceNumber;
        RepairOrderId = repairOrderId;
        DueDate = dueDate;
        Status = InvoiceStatus.Draft;
    }

    private Invoice()
    {
    }

    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid RepairOrderId { get; private set; }
    public DateTime DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }

    public IReadOnlyCollection<InvoiceLine> Lines => _lines.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public decimal TotalExcludingTax => _lines.Sum(l => l.TotalExcludingTax);
    public decimal TotalIncludingTax => _lines.Sum(l => l.TotalIncludingTax);
    public decimal TotalPaid => _payments.Sum(p => p.Amount);
    public decimal BalanceDue => TotalIncludingTax - TotalPaid;
    public bool IsFullyPaid => BalanceDue <= 0 && TotalIncludingTax > 0;

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static Invoice Create(string invoiceNumber, Guid repairOrderId, DateTime dueDate)
    {
        return new Invoice(Guid.NewGuid(), invoiceNumber, repairOrderId, dueDate);
    }

    public void AddLine(string description, int quantity, decimal unitPrice, decimal taxRate)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify lines of a non-draft invoice.");

        _lines.Add(new InvoiceLine(Guid.NewGuid(), description, quantity, unitPrice, taxRate));
    }

    public void FinalizeInvoice()
    {
        if (Status != InvoiceStatus.Draft) return;

        Status = InvoiceStatus.Unpaid;
    }

    public Result RegisterPayment(decimal amount, PaymentMethod method, string? reference, IDateTimeProvider dateTimeProvider)
    {
        if (Status == InvoiceStatus.Draft || Status == InvoiceStatus.Cancelled)
        {
            return Result.Failure(Error.Validation("Invoice.InvalidState", "Cannot pay a draft or cancelled invoice."));
        }

        if (amount <= 0)
        {
            return Result.Failure(Error.Validation("Payment.InvalidAmount", "Payment amount must be greater than zero."));
        }

        if (amount > BalanceDue)
        {
             return Result.Failure(Error.Validation("Payment.Overpayment", "Payment amount exceeds the balance due."));
        }

        _payments.Add(new Payment(Guid.NewGuid(), amount, dateTimeProvider.UtcNow, method, reference));

        if (BalanceDue <= 0)
        {
            Status = InvoiceStatus.Paid;
        }
        else
        {
            Status = InvoiceStatus.PartiallyPaid;
        }

        return Result.Success();
    }
}

public enum InvoiceStatus
{
    Draft = 1,
    Unpaid = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Cancelled = 5
}
