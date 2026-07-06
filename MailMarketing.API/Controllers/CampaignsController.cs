using System.Security.Claims;
using MailMarketing.API.Models.Campaigns;
using MailMarketing.Core.Utilities.Queue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMailQueue _mailQueue;

    public CampaignsController(IMailQueue mailQueue)
    {
        _mailQueue = mailQueue;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(SendCampaignRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();

        foreach (var subscriberId in request.SubscriberIds.Distinct())
        {
            await _mailQueue.EnqueueAsync(new MailQueueMessage
            {
                TemplateId = request.TemplateId,
                SubscriberId = subscriberId,
                UserId = userId
            }, cancellationToken);
        }

        return Accepted(new { Message = "Gönderim başlatıldı." });
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
