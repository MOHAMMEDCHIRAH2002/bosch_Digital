using MediatR;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.Domain.Commerce;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Commerce.Queries.GetEstimatePdf;

public sealed record GetEstimatePdfQuery(Guid EstimateId) : IQuery<byte[]>;

internal sealed class GetEstimatePdfQueryHandler : IQueryHandler<GetEstimatePdfQuery, byte[]>
{
    private readonly IPdfService _pdfService;
    // Real implementation would verify Estimate exists
    public GetEstimatePdfQueryHandler(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    public async Task<Result<byte[]>> Handle(GetEstimatePdfQuery request, CancellationToken cancellationToken)
    {
        var pdfBytes = await _pdfService.GenerateEstimatePdfAsync(request.EstimateId, cancellationToken);
        return pdfBytes;
    }
}
