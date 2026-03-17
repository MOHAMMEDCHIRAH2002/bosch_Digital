using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Domain.Workshop;

public sealed class RepairOrder : AggregateRoot, IAuditableEntity
{
    private readonly List<string> _intakePhotoUrls = new();

    private RepairOrder(Guid id, string orderNumber, Guid customerId, Guid vehicleId, RepairOrderType type, string visitReason, int? intakeMileage = null)
        : base(id)
    {
        OrderNumber = orderNumber;
        CustomerId = customerId;
        VehicleId = vehicleId;
        Type = type;
        VisitReason = visitReason;
        IntakeMileage = intakeMileage;
        Status = RepairOrderStatus.Draft;
    }

    private RepairOrder()
    {
    }

    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public Guid VehicleId { get; private set; }
    public RepairOrderType Type { get; private set; }
    public string VisitReason { get; private set; } = string.Empty;
    public int? IntakeMileage { get; private set; }
    public RepairOrderStatus Status { get; private set; }
    public IReadOnlyCollection<string> IntakePhotoUrls => _intakePhotoUrls.AsReadOnly();

    public DateTime CreatedOnUtc { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedOnUtc { get; set; }
    public string? ModifiedBy { get; set; }

    public static RepairOrder Create(string orderNumber, Guid customerId, Guid vehicleId, RepairOrderType type, string visitReason, int? intakeMileage = null)
    {
        return new RepairOrder(Guid.NewGuid(), orderNumber, customerId, vehicleId, type, visitReason, intakeMileage);
    }

    public void AddIntakePhoto(string photoUrl)
    {
        _intakePhotoUrls.Add(photoUrl);
    }

    public Result TransitionTo(RepairOrderStatus newStatus)
    {
        // Add robust state machine rules here. MVP: simplify.
        if (Status == RepairOrderStatus.Closed || Status == RepairOrderStatus.ClosedUnrepaired)
        {
            return Result.Failure(Error.Failure("RepairOrder.Closed", "Cannot change status of a closed repair order."));
        }

        Status = newStatus;
        return Result.Success();
    }
}

public enum RepairOrderType
{
    General = 1,
    ServiceRapide = 2,
    RetourTechnique = 3,
    Sinistre = 4
}

public enum RepairOrderStatus
{
    Draft = 1,
    Open = 2,
    AwaitingDiagnostic = 3,
    Diagnosing = 4,
    EstimateReady = 5,
    AwaitingCustomerApproval = 6,
    EstimateApproved = 7,
    EstimateRefused = 8,
    AwaitingParts = 9,
    PartsReserved = 10,
    InRepair = 11,
    Paused = 12,
    WaitingPart = 13,
    WaitingLeadDecision = 14,
    ExternalService = 15,
    RoadTestPending = 16,
    RepairCompleted = 17,
    QualityPending = 18,
    QualityValidated = 19,
    Invoiced = 20,
    Paid = 21,
    ReadyForDelivery = 22,
    Delivered = 23,
    Closed = 24,
    ClosedUnrepaired = 25
}

