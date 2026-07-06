using MailMarketing.Business.DTOs.Reports;
using MailMarketing.Business.Services;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Tests;

public sealed class ReportServiceTests
{
    [Fact]
    public async Task GetFilteredLogsAsyncReturnsOnlyCurrentUsersTemplateLogs()
    {
        await using var context = CreateContext();
        await SeedReportDataAsync(context);
        IMailLogService service = new MailLogService(context);

        var logs = await service.GetFilteredLogsAsync(10, new ReportFilterDto
        {
            Status = "Success",
            StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 1, 31, 23, 59, 59, DateTimeKind.Utc)
        });

        Assert.Single(logs);
        Assert.Equal(1, logs[0].Id);
        Assert.Equal(100, logs[0].TemplateId);
        Assert.Equal("Owner 10 Template", logs[0].TemplateTitle);
        Assert.Equal(500, logs[0].SubscriberId);
        Assert.Equal("subscriber@example.com", logs[0].SubscriberEmail);
        Assert.Equal("Success", logs[0].Status);
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MailMarketingDbContext(options);
    }

    private static async Task SeedReportDataAsync(MailMarketingDbContext context)
    {
        context.Users.AddRange(
            new User
            {
                Id = 10,
                FirstName = "Owner",
                LastName = "Ten",
                Email = "owner10@example.com",
                EncryptedPassword = "encrypted"
            },
            new User
            {
                Id = 20,
                FirstName = "Owner",
                LastName = "Twenty",
                Email = "owner20@example.com",
                EncryptedPassword = "encrypted"
            });

        context.Subscribers.AddRange(
            new Subscriber
            {
                Id = 500,
                Email = "subscriber@example.com"
            },
            new Subscriber
            {
                Id = 600,
                Email = "other@example.com"
            });

        context.Templates.AddRange(
            new Template
            {
                Id = 100,
                Title = "Owner 10 Template",
                Content = "Content",
                CreatedByUserId = 10
            },
            new Template
            {
                Id = 200,
                Title = "Owner 20 Template",
                Content = "Content",
                CreatedByUserId = 20
            });

        context.MailLogs.AddRange(
            new MailLog
            {
                Id = 1,
                TemplateId = 100,
                SubscriberId = 500,
                Status = "Success",
                SentAt = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)
            },
            new MailLog
            {
                Id = 2,
                TemplateId = 100,
                SubscriberId = 600,
                Status = "Failed",
                ErrorMessage = "SMTP error",
                SentAt = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)
            },
            new MailLog
            {
                Id = 3,
                TemplateId = 200,
                SubscriberId = 500,
                Status = "Success",
                SentAt = new DateTime(2026, 1, 15, 12, 0, 0, DateTimeKind.Utc)
            },
            new MailLog
            {
                Id = 4,
                TemplateId = 100,
                SubscriberId = 500,
                Status = "Success",
                SentAt = new DateTime(2026, 2, 15, 12, 0, 0, DateTimeKind.Utc)
            });

        await context.SaveChangesAsync();
    }
}
