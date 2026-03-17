using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Commands.AddIntakePhotos;

internal sealed class AddIntakePhotosCommandHandler : ICommandHandler<AddIntakePhotosCommand, bool>
{
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddIntakePhotosCommandHandler(
        IRepairOrderRepository repairOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _repairOrderRepository = repairOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AddIntakePhotosCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure<bool>(Error.NotFound("RepairOrder.NotFound", "Repair order not found."));
        }

        foreach (var photoUrl in request.PhotoUrls)
        {
            repairOrder.AddIntakePhoto(photoUrl);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
