namespace MailMarketing.Business.DTOs.EmailConfigs;

public class EmailConfigDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string MailServer { get; set; } = string.Empty;

    public int SmtpPort { get; set; }

    public bool UseSsl { get; set; }

    public string EmailAddress { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}
