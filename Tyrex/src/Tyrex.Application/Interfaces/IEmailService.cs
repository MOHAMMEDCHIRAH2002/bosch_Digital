namespace Tyrex.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken = default);
    Task SendWithAttachmentAsync(string recipientEmail, string subject, string body, string attachmentName, byte[] attachmentContent, CancellationToken cancellationToken = default);
}
