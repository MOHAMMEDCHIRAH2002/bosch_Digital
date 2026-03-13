using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Workshop;

public sealed class Diagnostic : AggregateRoot, IAuditableEntity
{
    private readonly List<string> _mediaUrls = new();

    private Diagnostic(Guid id, Guid repairOrderId, Guid technicianId, string notes)
        : base(id)
    {
        RepairOrderId = repairOrderId;
        TechnicianId = technicianId;
        Notes = notes;
        TechnicalValidationState = TechnicalValidationState.Pending;
    }

    private Diagnostic()
    {
    }

    public Guid RepairOrderId { get; private set; }
    public Guid TechnicianId { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public TechnicalValidationState TechnicalValidationState { get; private set; }
    
    public IReadOnlyCollection<string> MediaUrls => _mediaUrls.AsReadOnly();

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static Diagnostic Create(Guid repairOrderId, Guid technicianId, string notes)
    {
        return new Diagnostic(Guid.NewGuid(), repairOrderId, technicianId, notes);
    }

    public void AddMedia(string url)
    {
        _mediaUrls.Add(url);
    }

    public void ApproveTechnicalValidation()
    {
        TechnicalValidationState = TechnicalValidationState.Approved;
    }

    public void RejectTechnicalValidation(string reason)
    {
        TechnicalValidationState = TechnicalValidationState.Rejected;
        Notes += $"\n[Rejected: {reason}]";
    }
}

public enum TechnicalValidationState
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
