using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Commerce;

public sealed class Estimate : AggregateRoot, IAuditableEntity
{
    private readonly List<EstimateLineItem> _items = new();

    private Estimate(Guid id, Guid repairOrderId, int version)
        : base(id)
    {
        RepairOrderId = repairOrderId;
        Version = version;
        Status = EstimateStatus.Draft;
    }

    private Estimate()
    {
    }

    public Guid RepairOrderId { get; private set; }
    public int Version { get; private set; }
    public EstimateStatus Status { get; private set; }
    public string? ClientApprovalProofUrl { get; private set; }

    public IReadOnlyCollection<EstimateLineItem> Items => _items.AsReadOnly();

    public decimal TotalExcludingTax => _items.Sum(i => i.TotalExcludingTax);
    public decimal TotalIncludingTax => _items.Sum(i => i.TotalIncludingTax);

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static Estimate CreateInitial(Guid repairOrderId)
    {
        return new Estimate(Guid.NewGuid(), repairOrderId, 1);
    }

    public void AddLineItem(string description, int quantity, decimal unitPrice, decimal taxRate)
    {
        if (Status != EstimateStatus.Draft)
            throw new InvalidOperationException("Cannot modify an estimate that is not in draft status.");

        _items.Add(new EstimateLineItem(Guid.NewGuid(), description, quantity, unitPrice, taxRate));
    }

    public void SendToCustomer()
    {
        if (Status != EstimateStatus.Draft) return;
        Status = EstimateStatus.PendingApproval;
    }

    public void Approve(string proofUrl)
    {
        if (Status != EstimateStatus.PendingApproval) return;

        Status = EstimateStatus.Approved;
        ClientApprovalProofUrl = proofUrl;
        
        // MVP: Optionally raise DomainEvent (EstimateApprovedDomainEvent) to trigger part reservation
    }

    public void Refuse()
    {
        if (Status != EstimateStatus.PendingApproval) return;

        Status = EstimateStatus.Refused;
    }

    public Estimate CreateNextVersion()
    {
        if (Status == EstimateStatus.Draft)
            throw new InvalidOperationException("Current version is already a draft.");

        var newEstimate = new Estimate(Guid.NewGuid(), RepairOrderId, Version + 1);
        
        foreach (var item in _items)
        {
            newEstimate.AddLineItem(item.Description, item.Quantity, item.UnitPrice, item.TaxRate);
        }

        return newEstimate;
    }
}

public enum EstimateStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Refused = 4,
    Superseded = 5
}
