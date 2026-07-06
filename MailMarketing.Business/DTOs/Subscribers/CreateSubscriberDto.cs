using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Subscribers;

public class CreateSubscriberDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
