using MailMarketing.Core.Utilities.Entities;

namespace MailMarketing.Entities.Concrete;

public class User : IEntity
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EncryptedPassword { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<EmailConfig> EmailConfigs { get; set; } = new List<EmailConfig>();

    public ICollection<Template> Templates { get; set; } = new List<Template>();
}
