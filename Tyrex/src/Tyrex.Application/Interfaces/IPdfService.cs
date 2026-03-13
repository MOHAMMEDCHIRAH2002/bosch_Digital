namespace Tyrex.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateEstimatePdfAsync(Guid estimateId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
