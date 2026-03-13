using Tyrex.Application.Commerce.Interfaces;
using Tyrex.Application.Interfaces;
using Tyrex.Application.Messaging;
using Tyrex.SharedKernel.Primitives;

namespace Tyrex.Application.Commerce.Commands.SendEstimateEmail;

internal sealed class SendEstimateEmailCommandHandler : ICommandHandler<SendEstimateEmailCommand>
{
    private readonly IEstimateRepository _estimateRepository;
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;

    public SendEstimateEmailCommandHandler(
        IEstimateRepository estimateRepository,
        IPdfService pdfService,
        IEmailService emailService)
    {
        _estimateRepository = estimateRepository;
        _pdfService = pdfService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(SendEstimateEmailCommand request, CancellationToken cancellationToken)
    {
        var estimate = await _estimateRepository.GetByIdAsync(request.EstimateId, cancellationToken);
        if (estimate is null)
        {
            return Result.Failure(Error.NotFound("Estimate.NotFound", "The estimate was not found."));
        }

        var pdfBytes = await _pdfService.GenerateEstimatePdfAsync(request.EstimateId, cancellationToken);

        var subject = $"Your Estimate at TYREX (V{estimate.Version})";
        var body = $"Hello,\n\nPlease find attached your estimate for an amount of {estimate.TotalIncludingTax}.\n\nBest regards,\nTYREX Team";

        await _emailService.SendWithAttachmentAsync(
            request.ToEmail,
            subject,
            body,
            $"Estimate_V{estimate.Version}.pdf",
            pdfBytes,
            cancellationToken);

        return Result.Success();
    }
}
