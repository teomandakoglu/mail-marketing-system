using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class MailLog : IEntity
{
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public int SubscriberId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Template Template { get; set; } = null!;

    public Subscriber Subscriber { get; set; } = null!;
}
