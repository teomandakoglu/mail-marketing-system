using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Auth;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]
    public string NewPassword { get; set; } = string.Empty;
}
