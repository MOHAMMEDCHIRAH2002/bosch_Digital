using Tyrex.Application.Interfaces;

namespace Tyrex.Infrastructure.Services;

internal sealed class MockPdfService : IPdfService
{
    public Task<byte[]> GenerateEstimatePdfAsync(Guid estimateId, CancellationToken cancellationToken = default)
    {
         // MVP: Return a dummy byte array representing a PDF
         return Task.FromResult(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 });
    }

    public Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
         // MVP: Return a dummy byte array representing a PDF
         return Task.FromResult(new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E, 0x34 });
    }
}
