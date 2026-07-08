using MailMarketing.Business.Services;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Tests;

public sealed class DashboardServiceTests
{
    [Fact]
    public async Task DashboardStatsAreScopedToCurrentUser()
    {
        await using var context = CreateContext();
        context.Users.AddRange(
            new User
            {
                Id = 1,
                FirstName = "First",
                LastName = "User",
                Email = "first@example.com",
                EncryptedPassword = "encrypted"
            },
            new User
            {
                Id = 2,
                FirstName = "Second",
                LastName = "User",
                Email = "second@example.com",
                EncryptedPassword = "encrypted"
            });
        context.Subscribers.AddRange(
            new Subscriber { Id = 1, Email = "first-subscriber@example.com", UserId = 1 },
            new Subscriber { Id = 2, Email = "second-subscriber@example.com", UserId = 2 });
        context.Templates.AddRange(
            new Template { Id = 1, Title = "First Template", Content = "<p>First</p>", CreatedByUserId = 1 },
            new Template { Id = 2, Title = "Second Template", Content = "<p>Second</p>", CreatedByUserId = 2 });
        context.MailLogs.AddRange(
            new MailLog { TemplateId = 1, SubscriberId = 1, Status = "Success", SentAt = DateTime.UtcNow },
            new MailLog { TemplateId = 2, SubscriberId = 2, Status = "Success", SentAt = DateTime.UtcNow });
        await context.SaveChangesAsync();

        IDashboardService service = new DashboardService(context);

        var stats = await service.GetDashboardStatsAsync(1);

        Assert.Equal(1, stats.TotalSubscribers);
        Assert.Equal(1, stats.TotalTemplates);
        Assert.Equal(1, stats.TotalSentMails);
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MailMarketingDbContext(options);
    }
}
