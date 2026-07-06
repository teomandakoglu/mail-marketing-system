namespace MailMarketing.Business.DTOs.Templates;

public class TemplateDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }
}
