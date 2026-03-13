using MediatR;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Billing;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Billing.Queries.GetInvoicePdf;

public sealed record GetInvoicePdfQuery(Guid InvoiceId) : IQuery<byte[]>;

internal sealed class GetInvoicePdfQueryHandler : IQueryHandler<GetInvoicePdfQuery, byte[]>
{
    private readonly IPdfService _pdfService;
    
    public GetInvoicePdfQueryHandler(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    public async Task<Result<byte[]>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken)
    {
        var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(request.InvoiceId, cancellationToken);
        return pdfBytes;
    }
}
