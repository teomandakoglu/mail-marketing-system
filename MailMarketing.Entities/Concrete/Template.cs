using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class Template : IEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CreatedByUserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<MailLog> MailLogs { get; set; } = new List<MailLog>();
}
