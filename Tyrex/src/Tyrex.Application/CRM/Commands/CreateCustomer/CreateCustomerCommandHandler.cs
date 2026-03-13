using Tyrex.Application.CRM.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.CRM;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.CRM.Commands.CreateCustomer;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = request.Type == CustomerType.Individual
            ? Customer.CreateIndividual(request.FirstName, request.LastName, request.Email, request.Phone)
            : Customer.CreateCompany(request.CompanyName ?? string.Empty, request.FirstName, request.LastName, request.Email, request.Phone);

        _customerRepository.Add(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
