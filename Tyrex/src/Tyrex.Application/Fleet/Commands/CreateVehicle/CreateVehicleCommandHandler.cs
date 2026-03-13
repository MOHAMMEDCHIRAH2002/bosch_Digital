using Tyrex.Application.CRM.Interfaces;
using Tyrex.Application.Fleet.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Fleet;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Fleet.Commands.CreateVehicle;

internal sealed class CreateVehicleCommandHandler : ICommandHandler<CreateVehicleCommand, Guid>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleCommandHandler(
        IVehicleRepository vehicleRepository, 
        ICustomerRepository customerRepository, 
        IUnitOfWork unitOfWork)
    {
        _vehicleRepository = vehicleRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.Failure<Guid>(Error.NotFound("Customer.NotFound", "The specified customer was not found."));
        }

        var vehicle = Vehicle.Create(
            request.Vin,
            request.LicensePlate,
            request.Make,
            request.Model,
            request.Year,
            request.CustomerId,
            request.IsInternalFleet);

        _vehicleRepository.Add(vehicle);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}

