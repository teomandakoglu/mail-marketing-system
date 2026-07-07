using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Auth;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
