using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Quality;

public sealed class QualityChecklistItem : Entity
{
    internal QualityChecklistItem(Guid id, string name, string description)
        : base(id)
    {
        Name = name;
        Description = description;
        Status = QualityCheckItemStatus.Pending;
    }

    private QualityChecklistItem() { }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public QualityCheckItemStatus Status { get; private set; }
    public string? Notes { get; private set; }

    internal void UpdateStatus(QualityCheckItemStatus newStatus, string? notes)
    {
        Status = newStatus;
        Notes = notes;
    }
}

public enum QualityCheckItemStatus
{
    Pending = 1,
    Pass = 2,
    Fail = 3,
    NotApplicable = 4
}
