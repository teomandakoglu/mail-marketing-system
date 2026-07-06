using System.ComponentModel.DataAnnotations;

namespace MailMarketing.Business.DTOs.Templates;

public class CreateTemplateDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}
