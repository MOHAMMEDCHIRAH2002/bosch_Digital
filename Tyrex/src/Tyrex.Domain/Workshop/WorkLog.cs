using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Workshop;

public sealed class WorkLog : AggregateRoot, IAuditableEntity
{
    private WorkLog(Guid id, Guid repairOrderId, Guid technicianId, DateTime startTime)
        : base(id)
    {
        RepairOrderId = repairOrderId;
        TechnicianId = technicianId;
        StartTime = startTime;
        Status = WorkLogStatus.InProgress;
    }

    private WorkLog()
    {
    }

    public Guid RepairOrderId { get; private set; }
    public Guid TechnicianId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? PauseReason { get; private set; }
    public WorkLogStatus Status { get; private set; }

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static WorkLog Start(Guid repairOrderId, Guid technicianId, IDateTimeProvider dateTimeProvider)
    {
        return new WorkLog(Guid.NewGuid(), repairOrderId, technicianId, dateTimeProvider.UtcNow);
    }

    public void Complete(IDateTimeProvider dateTimeProvider)
    {
        if (Status != WorkLogStatus.InProgress) return;
        
        Status = WorkLogStatus.Completed;
        EndTime = dateTimeProvider.UtcNow;
    }

    public void Pause(string reason, IDateTimeProvider dateTimeProvider)
    {
        if (Status != WorkLogStatus.InProgress) return;

        Status = WorkLogStatus.Paused;
        PauseReason = reason;
        EndTime = dateTimeProvider.UtcNow;
    }
}

public enum WorkLogStatus
{
    InProgress = 1,
    Paused = 2,
    Completed = 3
}
