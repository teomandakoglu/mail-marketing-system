using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Auth;

public class CheckEmailDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
