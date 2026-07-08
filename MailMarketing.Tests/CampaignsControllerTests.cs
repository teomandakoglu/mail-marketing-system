using System.Security.Claims;
using MailMarketing.API.Controllers;
using MailMarketing.API.Models.Campaigns;
using MailMarketing.Core.Utilities.Queue;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Tests;

public sealed class CampaignsControllerTests
{
    [Fact]
    public async Task SendEnqueuesMessageForEachSubscriberAndReturnsAccepted()
    {
        var queue = new RecordingMailQueue();
        await using var context = CreateContext();
        await SeedCampaignDataAsync(context);
        var controller = CreateController(queue, context, 42);

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

    [Fact]
    public async Task SendRejectsEmptySubscriberSelection()
    {
        var queue = new RecordingMailQueue();
        await using var context = CreateContext();
        await SeedCampaignDataAsync(context);
        var controller = CreateController(queue, context, 42);

        var result = await controller.Send(new SendCampaignRequest
        {
            TemplateId = 9,
            SubscriberIds = new List<int>()
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Gönderim için en az bir abone seçilmelidir.", badRequest.Value);
        Assert.Empty(queue.Messages);
    }

    [Fact]
    public async Task SendRejectsInactiveTemplate()
    {
        var queue = new RecordingMailQueue();
        await using var context = CreateContext();
        await SeedCampaignDataAsync(context);
        var controller = CreateController(queue, context, 42);

        var result = await controller.Send(new SendCampaignRequest
        {
            TemplateId = 10,
            SubscriberIds = new List<int> { 3 }
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Aktif bir şablon seçilmelidir.", badRequest.Value);
        Assert.Empty(queue.Messages);
    }

    [Fact]
    public async Task SendRejectsSubscribersThatDoNotBelongToCurrentUser()
    {
        var queue = new RecordingMailQueue();
        await using var context = CreateContext();
        await SeedCampaignDataAsync(context);
        var controller = CreateController(queue, context, 42);

        var result = await controller.Send(new SendCampaignRequest
        {
            TemplateId = 9,
            SubscriberIds = new List<int> { 3, 99 }
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Seçilen aboneler geçersiz.", badRequest.Value);
        Assert.Empty(queue.Messages);
    }

    private static CampaignsController CreateController(RecordingMailQueue queue, MailMarketingDbContext context, int userId)
    {
        return new CampaignsController(queue, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
                }
            }
        };
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MailMarketingDbContext(options);
    }

    private static async Task SeedCampaignDataAsync(MailMarketingDbContext context)
    {
        context.Users.AddRange(
            new User
            {
                Id = 42,
                FirstName = "Current",
                LastName = "User",
                Email = "current@example.com",
                EncryptedPassword = "encrypted"
            },
            new User
            {
                Id = 43,
                FirstName = "Other",
                LastName = "User",
                Email = "other@example.com",
                EncryptedPassword = "encrypted"
            });
        context.Templates.AddRange(
            new Template
            {
                Id = 9,
                Title = "Active",
                Content = "<p>Active</p>",
                CreatedByUserId = 42,
                IsActive = true
            },
            new Template
            {
                Id = 10,
                Title = "Inactive",
                Content = "<p>Inactive</p>",
                CreatedByUserId = 42,
                IsActive = false
            },
            new Template
            {
                Id = 11,
                Title = "Other",
                Content = "<p>Other</p>",
                CreatedByUserId = 43,
                IsActive = true
            });
        context.Subscribers.AddRange(
            new Subscriber { Id = 3, Email = "first@example.com", UserId = 42 },
            new Subscriber { Id = 4, Email = "second@example.com", UserId = 42 },
            new Subscriber { Id = 5, Email = "third@example.com", UserId = 42 },
            new Subscriber { Id = 99, Email = "other-subscriber@example.com", UserId = 43 });
        await context.SaveChangesAsync();
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
