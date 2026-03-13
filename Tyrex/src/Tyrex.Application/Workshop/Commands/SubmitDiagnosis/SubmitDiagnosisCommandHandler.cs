using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Commands.SubmitDiagnosis;

internal sealed class SubmitDiagnosisCommandHandler : ICommandHandler<SubmitDiagnosisCommand, Guid>
{
    private readonly IDiagnosticRepository _diagnosticRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitDiagnosisCommandHandler(
        IDiagnosticRepository diagnosticRepository,
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _diagnosticRepository = diagnosticRepository;
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SubmitDiagnosisCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        
        if (repairOrder is null)
        {
            return Result.Failure<Guid>(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
        }

        // MVP: Ensure status transition allows diagnosis
        var transitionResult = repairOrder.TransitionTo(RepairOrderStatus.Diagnosing);
        if (transitionResult.IsFailure)
        {
            return Result.Failure<Guid>(transitionResult.Error);
        }

        var diagnostic = Diagnostic.Create(request.RepairOrderId, request.TechnicianId, request.Notes);
        
        foreach(var url in request.MediaUrls)
        {
            diagnostic.AddMedia(url);
        }

        _diagnosticRepository.Add(diagnostic);

        // Transition Repair Order to Estimate Ready after successful diagnosis.
        repairOrder.TransitionTo(RepairOrderStatus.EstimateReady);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return diagnostic.Id;
    }
}

