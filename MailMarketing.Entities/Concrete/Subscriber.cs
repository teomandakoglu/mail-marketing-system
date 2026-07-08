using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class Subscriber : IEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }

    public ICollection<MailLog> MailLogs { get; set; } = new List<MailLog>();
}
