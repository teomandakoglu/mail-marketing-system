namespace MailMarketing.API.Models.Campaigns;

public class SendCampaignRequest
{
    public int TemplateId { get; set; }

    public List<int> SubscriberIds { get; set; } = new();
}
