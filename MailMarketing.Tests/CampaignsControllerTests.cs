using System.Security.Claims;
using MailMarketing.API.Controllers;
using MailMarketing.API.Models.Campaigns;
using MailMarketing.Core.Utilities.Queue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.Tests;

public sealed class CampaignsControllerTests
{
    [Fact]
    public async Task SendEnqueuesMessageForEachSubscriberAndReturnsAccepted()
    {
        var queue = new RecordingMailQueue();
        var controller = new CampaignsController(queue)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "42")
                    }, "TestAuth"))
                }
            }
        };

        var result = await controller.Send(new SendCampaignRequest
        {
            TemplateId = 9,
            SubscriberIds = new List<int> { 3, 4, 5 }
        });

        var acceptedResult = Assert.IsType<AcceptedResult>(result);
        Assert.NotNull(acceptedResult.Value);
        Assert.Equal(3, queue.Messages.Count);
        Assert.All(queue.Messages, message =>
        {
            Assert.Equal(9, message.TemplateId);
            Assert.Equal(42, message.UserId);
        });
        Assert.Equal(new[] { 3, 4, 5 }, queue.Messages.Select(message => message.SubscriberId));
    }

    private sealed class RecordingMailQueue : IMailQueue
    {
        public List<MailQueueMessage> Messages { get; } = new();

        public ValueTask EnqueueAsync(MailQueueMessage message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);

            return ValueTask.CompletedTask;
        }

        public ValueTask<MailQueueMessage> DequeueAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}
