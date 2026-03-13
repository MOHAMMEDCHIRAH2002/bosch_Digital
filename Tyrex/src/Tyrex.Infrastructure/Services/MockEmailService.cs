using Tyrex.Application.Interfaces;

namespace Tyrex.Infrastructure.Services;

internal sealed class MockEmailService : IEmailService
{
    public Task SendAsync(string recipientEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        // MVP: Console logging to pretend sending email
        Console.WriteLine($"[EMAIL] To: {recipientEmail} | Subject: {subject}");
        Console.WriteLine($"[EMAIL] Body: {body}");
        return Task.CompletedTask;
    }

    public Task SendWithAttachmentAsync(string recipientEmail, string subject, string body, string attachmentName, byte[] attachmentContent, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[EMAIL] To: {recipientEmail} | Subject: {subject} | Attachment: {attachmentName} ({attachmentContent.Length} bytes)");
        Console.WriteLine($"[EMAIL] Body: {body}");
        return Task.CompletedTask;
    }
}
