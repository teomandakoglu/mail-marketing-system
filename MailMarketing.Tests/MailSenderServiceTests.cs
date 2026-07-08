using MailMarketing.Business.Services;
using MailMarketing.Core.Utilities.Queue;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class MailSenderServiceTests
{
    [Fact]
    public async Task SendAsyncWritesTurkishFailureStatusWhenEmailConfigIsMissing()
    {
        await using var context = CreateContext();
        await SeedMailDataAsync(context);
        IMailSenderService service = new MailSenderService(context, CreateEncryptionService(), CreateConfiguration());

        await service.SendAsync(new MailQueueMessage
        {
            UserId = 1,
            TemplateId = 10,
            SubscriberId = 20
        });

        var log = await context.MailLogs.SingleAsync();
        Assert.Equal("Başarısız", log.Status);
        Assert.Equal("Email configuration not found.", log.ErrorMessage);
    }

    [Fact]
    public async Task SendAsyncUsesApplicationWideSubscribers()
    {
        await using var context = CreateContext();
        await SeedMailDataAsync(context);
        context.Subscribers.Add(new Subscriber
        {
            Id = 30,
            Email = "public@example.com",
            UserId = 2
        });
        await context.SaveChangesAsync();
        IMailSenderService service = new MailSenderService(context, CreateEncryptionService(), CreateConfiguration());

        await service.SendAsync(new MailQueueMessage
        {
            UserId = 1,
            TemplateId = 10,
            SubscriberId = 30
        });

        var log = await context.MailLogs.SingleAsync();
        Assert.Equal(30, log.SubscriberId);
        Assert.Equal("Başarısız", log.Status);
        Assert.Equal("Email configuration not found.", log.ErrorMessage);
    }

    [Fact]
    public void ApiConfigurationDefinesMailSendingTimeout()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetApiProjectPath())
            .AddJsonFile("appsettings.json")
            .Build();

        Assert.Equal(30, configuration.GetValue<int>("MailSending:TimeoutSeconds"));
    }

    private static async Task SeedMailDataAsync(MailMarketingDbContext context)
    {
        context.Users.Add(new User
        {
            Id = 1,
            FirstName = "Sender",
            LastName = "User",
            Email = "sender@example.com",
            EncryptedPassword = "encrypted"
        });
        context.Users.Add(new User
        {
            Id = 2,
            FirstName = "Public",
            LastName = "Owner",
            Email = "public-owner@example.com",
            EncryptedPassword = "encrypted"
        });
        context.Templates.Add(new Template
        {
            Id = 10,
            Title = "Template",
            Content = "<p>Hello</p>",
            CreatedByUserId = 1,
            IsActive = true
        });
        context.Subscribers.Add(new Subscriber
        {
            Id = 20,
            Email = "subscriber@example.com",
            UserId = 1
        });
        await context.SaveChangesAsync();
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MailMarketingDbContext(options);
    }

    private static IEncryptionService CreateEncryptionService()
    {
        return new EncryptionService(CreateConfiguration());
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:SecretKey"] = "12345678901234567890123456789012",
                ["MailSending:TimeoutSeconds"] = "30"
            })
            .Build();
    }

    private static string GetApiProjectPath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null && !File.Exists(Path.Combine(currentDirectory.FullName, "MailMarketingSystem.sln")))
        {
            currentDirectory = currentDirectory.Parent;
        }

        Assert.NotNull(currentDirectory);

        return Path.Combine(currentDirectory.FullName, "MailMarketing.API");
    }
}
