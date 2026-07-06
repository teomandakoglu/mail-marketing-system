namespace MailMarketing.Core.Utilities.Queue;

public class MailQueueMessage
{
    public int TemplateId { get; set; }

    public int SubscriberId { get; set; }

    public int UserId { get; set; }
}
