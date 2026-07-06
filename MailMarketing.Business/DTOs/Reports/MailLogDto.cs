namespace MailMarketing.Business.DTOs.Reports;

public class MailLogDto
{
    public int Id { get; set; }

    public int TemplateId { get; set; }

    public string TemplateTitle { get; set; } = string.Empty;

    public int SubscriberId { get; set; }

    public string SubscriberEmail { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public DateTime SentAt { get; set; }
}
