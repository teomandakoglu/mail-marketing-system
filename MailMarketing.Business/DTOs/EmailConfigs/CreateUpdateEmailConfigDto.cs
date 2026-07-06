using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.EmailConfigs;

public class CreateUpdateEmailConfigDto
{
    [Required]
    [MaxLength(200)]
    public string MailServer { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int SmtpPort { get; set; }

    public bool UseSsl { get; set; }

    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
