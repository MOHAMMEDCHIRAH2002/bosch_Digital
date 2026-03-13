using Tyrex.Application.Billing.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Interfaces;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Billing.Commands.RegisterPayment;

internal sealed class RegisterPaymentCommandHandler : ICommandHandler<RegisterPaymentCommand>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterPaymentCommandHandler(
        IInvoiceRepository invoiceRepository,
        IRepairOrderRepository repairOrderRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _repairOrderRepository = repairOrderRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            return Result.Failure(Error.NotFound("Invoice.NotFound", "The invoice was not found."));
        }

        var result = invoice.RegisterPayment(request.Amount, request.Method, request.ReferenceInfo, _dateTimeProvider);
        if (result.IsFailure) return result;

        if (invoice.IsFullyPaid)
        {
             var repairOrder = await _repairOrderRepository.GetByIdAsync(invoice.RepairOrderId, cancellationToken);
             if (repairOrder is not null)
             {
                 repairOrder.TransitionTo(RepairOrderStatus.Delivered);
             }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
