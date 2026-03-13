using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Quality.Interfaces;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Quality;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Quality.Commands.SubmitQualityChecklist;

internal sealed class SubmitQualityChecklistCommandHandler : ICommandHandler<SubmitQualityChecklistCommand, Guid>
{
    private readonly IQualityChecklistRepository _qualityChecklistRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitQualityChecklistCommandHandler(
        IQualityChecklistRepository qualityChecklistRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _qualityChecklistRepository = qualityChecklistRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SubmitQualityChecklistCommand request, CancellationToken cancellationToken)
    {
         var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
         if (repairOrder is null)
         {
             return Result.Failure<Guid>(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
         }

         if (repairOrder.Status != RepairOrderStatus.RepairCompleted && repairOrder.Status != RepairOrderStatus.QualityPending)
         {
             return Result.Failure<Guid>(Error.Validation("RepairOrder.InvalidState", "Repair order must be completed before quality check."));
         }

         var templates = request.Items.Select(i => (i.Name, i.Description));
         var checklist = QualityChecklist.Create(request.RepairOrderId, request.InspectorId, templates);

         foreach (var itemResult in request.Items)
         {
             var item = checklist.Items.First(i => i.Name == itemResult.Name);
             var updateResult = checklist.UpdateItemStatus(item.Id, itemResult.Status, itemResult.Notes);
             if (updateResult.IsFailure) return Result.Failure<Guid>(updateResult.Error);
         }

         var submitResult = checklist.Submit(request.FinalNotes);
         if (submitResult.IsFailure) return Result.Failure<Guid>(submitResult.Error);

         _qualityChecklistRepository.Add(checklist);

         if (checklist.Status == QualityChecklistStatus.Passed)
         {
             repairOrder.TransitionTo(RepairOrderStatus.QualityValidated);
         }
         else
         {
             // MVP: If failed, we might transition it to a "Paused" or "ExternalService" state depending on rules. Sticking to simple transition.
             // Leaving it QualityPending for MVP so technician sees it needs fixing.
         }

         await _unitOfWork.SaveChangesAsync(cancellationToken);

         return checklist.Id;
    }
}
