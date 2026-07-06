using System.Threading.Channels;

namespace MailMarketing.Core.Utilities.Queue;

public class MailQueue : IMailQueue
{
    private readonly Channel<MailQueueMessage> _channel = Channel.CreateUnbounded<MailQueueMessage>();

    public async ValueTask EnqueueAsync(MailQueueMessage message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        await _channel.Writer.WriteAsync(message, cancellationToken);
    }

    public async ValueTask<MailQueueMessage> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _channel.Reader.ReadAsync(cancellationToken);
    }
}
