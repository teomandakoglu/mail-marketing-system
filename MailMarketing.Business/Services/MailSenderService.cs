using System.Net;
using System.Net.Mail;
using MailMarketing.Core.Utilities.Queue;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class MailSenderService : IMailSenderService
{
    private readonly MailMarketingDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public MailSenderService(MailMarketingDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task SendAsync(MailQueueMessage message, CancellationToken cancellationToken = default)
    {
        var template = await _context.Templates
            .SingleOrDefaultAsync(item => item.Id == message.TemplateId && item.CreatedByUserId == message.UserId, cancellationToken);
        var subscriber = await _context.Subscribers
            .SingleOrDefaultAsync(item => item.Id == message.SubscriberId && item.UserId == message.UserId, cancellationToken);

        if (template is null || subscriber is null)
        {
            return;
        }

        var emailConfig = await _context.EmailConfigs
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UserId == message.UserId, cancellationToken);

        if (emailConfig is null)
        {
            await AddMailLogAsync(template.Id, subscriber.Id, "Failed", "Email configuration not found.", cancellationToken);
            return;
        }

        try
        {
            using var mailMessage = BuildMailMessage(emailConfig, template, subscriber);
            using var smtpClient = BuildSmtpClient(emailConfig);

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            await AddMailLogAsync(template.Id, subscriber.Id, "Success", null, cancellationToken);
        }
        catch (Exception exception)
        {
            await AddMailLogAsync(template.Id, subscriber.Id, "Failed", exception.Message, cancellationToken);
        }
    }

    private MailMessage BuildMailMessage(EmailConfig emailConfig, Template template, Subscriber subscriber)
    {
        return new MailMessage
        {
            From = new MailAddress(emailConfig.EmailAddress),
            Subject = template.Title,
            Body = template.Content,
            IsBodyHtml = true
        }.Also(message => message.To.Add(subscriber.Email));
    }

    private SmtpClient BuildSmtpClient(EmailConfig emailConfig)
    {
        return new SmtpClient(emailConfig.MailServer, emailConfig.SmtpPort)
        {
            EnableSsl = emailConfig.UseSsl,
            Credentials = new NetworkCredential(
                emailConfig.EmailAddress,
                _encryptionService.Decrypt(emailConfig.EncryptedPassword))
        };
    }

    private async Task AddMailLogAsync(
        int templateId,
        int subscriberId,
        string status,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        await _context.MailLogs.AddAsync(new MailLog
        {
            TemplateId = templateId,
            SubscriberId = subscriberId,
            Status = status,
            ErrorMessage = errorMessage,
            SentAt = DateTime.UtcNow
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}

internal static class MailMessageExtensions
{
    public static MailMessage Also(this MailMessage message, Action<MailMessage> action)
    {
        action(message);

        return message;
    }
}
