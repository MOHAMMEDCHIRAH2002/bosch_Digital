using Tyrex.Application.CRM.Interfaces;
using Tyrex.Application.Fleet.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Workshop.Commands.CreateRepairOrder;

internal sealed class CreateRepairOrderCommandHandler : ICommandHandler<CreateRepairOrderCommand, Guid>
{
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRepairOrderCommandHandler(
        IRepairOrderRepository repairOrderRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository,
        IUnitOfWork unitOfWork)
    {
        _repairOrderRepository = repairOrderRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateRepairOrderCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure<Guid>(Error.NotFound("Customer.NotFound", "Customer not found."));
        }

        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId, cancellationToken);
        if (vehicle is null || vehicle.CustomerId != request.CustomerId)
        {
            return Result.Failure<Guid>(Error.Validation("Vehicle.Invalid", "Vehicle not found or does not belong to customer."));
        }

        var nextOrderNumber = await _repairOrderRepository.GenerateNextOrderNumberAsync(cancellationToken);

        var repairOrder = RepairOrder.Create(
            nextOrderNumber,
            request.CustomerId,
            request.VehicleId,
            request.Type,
            request.VisitReason,
            request.IntakeMileage);

        _repairOrderRepository.Add(repairOrder);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return repairOrder.Id;
    }
}

