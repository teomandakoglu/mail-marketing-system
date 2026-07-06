using MailMarketing.Core.Utilities.Queue;

namespace MailMarketing.Tests;

public sealed class MailQueueTests
{
    [Fact]
    public async Task MailQueueDequeuesMessagesInEnqueueOrder()
    {
        IMailQueue queue = new MailQueue();
        var first = new MailQueueMessage { TemplateId = 1, SubscriberId = 10, UserId = 100 };
        var second = new MailQueueMessage { TemplateId = 2, SubscriberId = 20, UserId = 200 };

        await queue.EnqueueAsync(first);
        await queue.EnqueueAsync(second);

        var dequeuedFirst = await queue.DequeueAsync(CancellationToken.None);
        var dequeuedSecond = await queue.DequeueAsync(CancellationToken.None);

        Assert.Equal(first.TemplateId, dequeuedFirst.TemplateId);
        Assert.Equal(first.SubscriberId, dequeuedFirst.SubscriberId);
        Assert.Equal(first.UserId, dequeuedFirst.UserId);
        Assert.Equal(second.TemplateId, dequeuedSecond.TemplateId);
        Assert.Equal(second.SubscriberId, dequeuedSecond.SubscriberId);
        Assert.Equal(second.UserId, dequeuedSecond.UserId);
    }
}
