using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Commands.CompleteRepair;

internal sealed class CompleteRepairCommandHandler : ICommandHandler<CompleteRepairCommand>
{
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IWorkLogRepository _workLogRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteRepairCommandHandler(
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

    public async Task<Result> Handle(CompleteRepairCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
        }

        var activeLog = await _workLogRepository.GetActiveLogForRepairOrderAsync(request.RepairOrderId, cancellationToken);
        if (activeLog is null)
        {
            return Result.Failure(Error.NotFound("WorkLog.NotFound", "There is no active work log for this repair order."));
        }

        if (activeLog.TechnicianId != request.TechnicianId)
        {
             return Result.Failure(Error.Failure("WorkLog.Unauthorized", "Only the assigned technician can complete this task."));
        }

        activeLog.Complete(_dateTimeProvider);
        
        var transitionResult = repairOrder.TransitionTo(RepairOrderStatus.RepairCompleted);
        if (transitionResult.IsFailure) return transitionResult;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

