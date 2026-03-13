using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Commands.StartRepair;

internal sealed class StartRepairCommandHandler : ICommandHandler<StartRepairCommand, Guid>
{
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IWorkLogRepository _workLogRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public StartRepairCommandHandler(
        IRepairOrderRepository repairOrderRepository,
        IWorkLogRepository workLogRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _repairOrderRepository = repairOrderRepository;
        _workLogRepository = workLogRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(StartRepairCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure<Guid>(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
        }

        var activeLog = await _workLogRepository.GetActiveLogForRepairOrderAsync(request.RepairOrderId, cancellationToken);
        if (activeLog is not null)
        {
            return Result.Failure<Guid>(Error.Failure("WorkLog.AlreadyActive", "There is already an active work log for this repair order."));
        }

        var transitionResult = repairOrder.TransitionTo(RepairOrderStatus.InRepair);
        if (transitionResult.IsFailure) return Result.Failure<Guid>(transitionResult.Error);

        var workLog = WorkLog.Start(request.RepairOrderId, request.TechnicianId, _dateTimeProvider);
        _workLogRepository.Add(workLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return workLog.Id;
    }
}

