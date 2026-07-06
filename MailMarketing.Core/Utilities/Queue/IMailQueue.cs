namespace MailMarketing.Core.Utilities.Queue;

public interface IMailQueue
{
    ValueTask EnqueueAsync(MailQueueMessage message, CancellationToken cancellationToken = default);

    ValueTask<MailQueueMessage> DequeueAsync(CancellationToken cancellationToken);
}
