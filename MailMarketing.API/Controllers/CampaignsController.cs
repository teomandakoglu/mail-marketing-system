using System.Security.Claims;
using MailMarketing.API.Models.Campaigns;
using MailMarketing.Core.Utilities.Queue;
using MailMarketing.DataAccess.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMailQueue _mailQueue;
    private readonly MailMarketingDbContext _context;

    public CampaignsController(IMailQueue mailQueue, MailMarketingDbContext context)
    {
        _mailQueue = mailQueue;
        _context = context;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(SendCampaignRequest request, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var distinctSubscriberIds = request.SubscriberIds.Distinct().ToList();

        if (request.TemplateId <= 0)
        {
            return BadRequest("Şablon seçilmelidir.");
        }

        if (distinctSubscriberIds.Count == 0)
        {
            return BadRequest("Gönderim için en az bir abone seçilmelidir.");
        }

        var templateIsActive = await _context.Templates
            .AnyAsync(template =>
                template.Id == request.TemplateId &&
                template.CreatedByUserId == userId &&
                template.IsActive,
                cancellationToken);

        if (!templateIsActive)
        {
            return BadRequest("Aktif bir şablon seçilmelidir.");
        }

        var validSubscriberCount = await _context.Subscribers
            .CountAsync(subscriber =>
                distinctSubscriberIds.Contains(subscriber.Id),
                cancellationToken);

        if (validSubscriberCount != distinctSubscriberIds.Count)
        {
            return BadRequest("Seçilen aboneler geçersiz.");
        }

        foreach (var subscriberId in distinctSubscriberIds)
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
