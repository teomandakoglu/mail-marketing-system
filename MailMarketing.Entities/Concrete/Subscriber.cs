using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class Subscriber : IEntity
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MailLog> MailLogs { get; set; } = new List<MailLog>();
}
