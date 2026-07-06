using MailMarketing.Core.Utilities.Queue;

namespace MailMarketing.Business.Services;

public interface IMailSenderService
{
    Task SendAsync(MailQueueMessage message, CancellationToken cancellationToken = default);
}
