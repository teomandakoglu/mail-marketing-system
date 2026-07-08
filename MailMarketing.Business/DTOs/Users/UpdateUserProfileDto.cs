using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Users;

public class UpdateUserProfileDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]
    public string Password { get; set; } = string.Empty;
}
