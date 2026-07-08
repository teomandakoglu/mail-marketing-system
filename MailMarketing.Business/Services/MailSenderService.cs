using System.Net;
using System.Net.Mail;
using MailMarketing.Core.Utilities.Queue;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Business.Services;

public class MailSenderService : IMailSenderService
{
    private readonly MailMarketingDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly IConfiguration _configuration;

    public MailSenderService(
        MailMarketingDbContext context,
        IEncryptionService encryptionService,
        IConfiguration configuration)
    {
        _context = context;
        _encryptionService = encryptionService;
        _configuration = configuration;
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
            await AddMailLogAsync(template.Id, subscriber.Id, "Başarısız", "Email configuration not found.", cancellationToken);
            return;
        }

        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(GetTimeout());
            using var mailMessage = BuildMailMessage(emailConfig, template, subscriber);
            using var smtpClient = BuildSmtpClient(emailConfig);

            await smtpClient.SendMailAsync(mailMessage, timeoutCts.Token);
            await AddMailLogAsync(template.Id, subscriber.Id, "Başarılı", null, cancellationToken);
        }
        catch (Exception exception)
        {
            await AddMailLogAsync(template.Id, subscriber.Id, "Başarısız", exception.Message, cancellationToken);
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
            Timeout = (int)GetTimeout().TotalMilliseconds,
            Credentials = new NetworkCredential(
                emailConfig.EmailAddress,
                _encryptionService.Decrypt(emailConfig.EncryptedPassword))
        };
    }

    private TimeSpan GetTimeout()
    {
        var configuredValue = _configuration["MailSending:TimeoutSeconds"];
        var timeoutSeconds = int.TryParse(configuredValue, out var parsedTimeout) ? parsedTimeout : 30;

        return TimeSpan.FromSeconds(Math.Max(1, timeoutSeconds));
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
