using Tyrex.Application.Billing.Interfaces;
using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Application.Workshop.Interfaces;
using Tyrex.Domain.Billing;
using Tyrex.Domain.Workshop;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Billing.Commands.GenerateInvoice;

internal sealed class GenerateInvoiceCommandHandler : ICommandHandler<GenerateInvoiceCommand, Guid>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IRepairOrderRepository _repairOrderRepository;
    private readonly IEstimateRepository _estimateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateInvoiceCommandHandler(
        IInvoiceRepository invoiceRepository,
        IRepairOrderRepository repairOrderRepository,
        IEstimateRepository estimateRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _repairOrderRepository = repairOrderRepository;
        _estimateRepository = estimateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var repairOrder = await _repairOrderRepository.GetByIdAsync(request.RepairOrderId, cancellationToken);
        if (repairOrder is null)
        {
            return Result.Failure<Guid>(Error.NotFound("RepairOrder.NotFound", "The repair order was not found."));
        }

        if (repairOrder.Status < RepairOrderStatus.QualityValidated)
        {
             return Result.Failure<Guid>(Error.Validation("RepairOrder.InvalidState", "Repair order must pass quality check before invoicing."));
        }

        // Generate from the active estimate line items
        var estimate = await _estimateRepository.GetActiveByRepairOrderIdAsync(request.RepairOrderId, cancellationToken);
        if (estimate is null || estimate.Status != Domain.Commerce.EstimateStatus.Approved)
        {
             return Result.Failure<Guid>(Error.Validation("Estimate.Invalid", "Cannot invoice without an approved estimate."));
        }

        var nextInvoiceNumber = await _invoiceRepository.GenerateNextInvoiceNumberAsync(cancellationToken);
        
        var invoice = Invoice.Create(nextInvoiceNumber, request.RepairOrderId, request.DueDate);

        foreach (var item in estimate.Items)
        {
            invoice.AddLine(item.Description, item.Quantity, item.UnitPrice, item.TaxRate);
        }

        invoice.FinalizeInvoice();
        repairOrder.TransitionTo(RepairOrderStatus.Invoiced);

        _invoiceRepository.Add(invoice);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return invoice.Id;
    }
}
