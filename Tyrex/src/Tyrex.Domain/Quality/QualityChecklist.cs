using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Quality;

public sealed class QualityChecklist : AggregateRoot, IAuditableEntity
{
    private readonly List<QualityChecklistItem> _items = new();

    private QualityChecklist(Guid id, Guid repairOrderId, Guid inspectorId)
        : base(id)
    {
        RepairOrderId = repairOrderId;
        InspectorId = inspectorId;
        Status = QualityChecklistStatus.Draft;
    }

    private QualityChecklist()
    {
    }

    public Guid RepairOrderId { get; private set; }
    public Guid InspectorId { get; private set; }
    public QualityChecklistStatus Status { get; private set; }
    public string? FinalNotes { get; private set; }

    public IReadOnlyCollection<QualityChecklistItem> Items => _items.AsReadOnly();

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static QualityChecklist Create(Guid repairOrderId, Guid inspectorId, IEnumerable<(string Name, string Description)> templateItems)
    {
        var checklist = new QualityChecklist(Guid.NewGuid(), repairOrderId, inspectorId);
        
        foreach (var item in templateItems)
        {
            checklist._items.Add(new QualityChecklistItem(Guid.NewGuid(), item.Name, item.Description));
        }

        return checklist;
    }

    public Result UpdateItemStatus(Guid itemId, QualityCheckItemStatus status, string? notes)
    {
        if (Status != QualityChecklistStatus.Draft)
        {
             return Result.Failure(Error.Validation("QualityChecklist.NotDraft", "Cannot modify items of a submitted checklist."));
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return Result.Failure(Error.NotFound("QualityChecklistItem.NotFound", "The item was not found in this checklist."));
        }

        item.UpdateStatus(status, notes);
        return Result.Success();
    }

    public Result Submit(string? finalNotes)
    {
        if (Status != QualityChecklistStatus.Draft)
        {
             return Result.Failure(Error.Validation("QualityChecklist.AlreadySubmitted", "This checklist has already been submitted."));
        }

        if (_items.Any(i => i.Status == QualityCheckItemStatus.Pending))
        {
            return Result.Failure(Error.Validation("QualityChecklist.Incomplete", "All items must be evaluated before submitting."));
        }

        FinalNotes = finalNotes;
        
        bool hasFailures = _items.Any(i => i.Status == QualityCheckItemStatus.Fail);
        Status = hasFailures ? QualityChecklistStatus.Failed : QualityChecklistStatus.Passed;

        return Result.Success();
    }
}

public enum QualityChecklistStatus
{
    Draft = 1,
    Passed = 2,
    Failed = 3
}
