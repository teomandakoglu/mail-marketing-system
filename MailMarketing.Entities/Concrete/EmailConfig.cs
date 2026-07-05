using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class EmailConfig : IEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string MailServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; }

    public bool UseSsl { get; set; }

    public string EmailAddress { get; set; } = string.Empty;

    public string EncryptedPassword { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
