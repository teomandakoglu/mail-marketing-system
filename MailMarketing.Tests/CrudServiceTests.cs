using MailMarketing.Business.DTOs.EmailConfigs;
using MailMarketing.Business.DTOs.Subscribers;
using MailMarketing.Business.DTOs.Templates;
using MailMarketing.Business.Services;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class CrudServiceTests
{
    [Fact]
    public async Task SubscriberServiceRejectsDuplicateEmail()
    {
        await using var context = CreateContext();
        ISubscriberService service = new SubscriberService(context);

        var first = await service.AddAsync(new CreateSubscriberDto { Email = "member@example.com" });
        var second = await service.AddAsync(new CreateSubscriberDto { Email = "MEMBER@example.com" });
        var subscribers = await service.GetAllAsync();

        Assert.True(first);
        Assert.False(second);
        Assert.Single(subscribers);
    }

    [Fact]
    public async Task TemplateServiceCreatesUpdatesAndDeletesTemplate()
    {
        await using var context = CreateContext();
        context.Users.Add(new User
        {
            Id = 7,
            FirstName = "Template",
            LastName = "Owner",
            Email = "owner@example.com",
            EncryptedPassword = "encrypted"
        });
        await context.SaveChangesAsync();

        ITemplateService service = new TemplateService(context);

        var created = await service.AddAsync(new CreateTemplateDto
        {
            Title = "Welcome",
            Content = "<p>Hello</p>"
        }, 7);

        var updated = await service.UpdateAsync(created.Id, new UpdateTemplateDto
        {
            Title = "Updated",
            Content = "<p>Updated</p>",
            IsActive = false
        }, 7);

        var loaded = await service.GetByIdAsync(created.Id, 7);
        var deleted = await service.DeleteAsync(created.Id, 7);

        Assert.Equal("Welcome", created.Title);
        Assert.True(updated);
        Assert.NotNull(loaded);
        Assert.Equal("Updated", loaded.Title);
        Assert.False(loaded.IsActive);
        Assert.True(deleted);
        Assert.Empty(await service.GetAllAsync(7));
    }

    [Fact]
    public async Task TemplateServiceReturnsOnlyCurrentUsersTemplates()
    {
        await using var context = CreateContext();
        context.Users.AddRange(
            new User
            {
                Id = 7,
                FirstName = "Template",
                LastName = "Owner",
                Email = "owner@example.com",
                EncryptedPassword = "encrypted"
            },
            new User
            {
                Id = 8,
                FirstName = "Other",
                LastName = "Owner",
                Email = "other@example.com",
                EncryptedPassword = "encrypted"
            });
        context.Templates.AddRange(
            new Template
            {
                Title = "Mine",
                Content = "<p>Mine</p>",
                CreatedByUserId = 7
            },
            new Template
            {
                Title = "Not Mine",
                Content = "<p>Not Mine</p>",
                CreatedByUserId = 8
            });
        await context.SaveChangesAsync();

        ITemplateService service = new TemplateService(context);

        var templates = await service.GetAllAsync(7);
        var hiddenTemplate = await service.GetByIdAsync(2, 7);

        Assert.Single(templates);
        Assert.Equal("Mine", templates[0].Title);
        Assert.Null(hiddenTemplate);
    }

    [Fact]
    public async Task EmailConfigServiceEncryptsPasswordBeforeSaving()
    {
        await using var context = CreateContext();
        var encryptionService = CreateEncryptionService();
        IEmailConfigService service = new EmailConfigService(context, encryptionService);

        var dto = new CreateUpdateEmailConfigDto
        {
            MailServer = "smtp.example.com",
            SmtpPort = 587,
            UseSsl = true,
            EmailAddress = "sender@example.com",
            Password = "SmtpPassword123"
        };

        var saved = await service.CreateOrUpdateAsync(3, dto);
        var entity = await context.EmailConfigs.SingleAsync();

        Assert.Equal("smtp.example.com", saved.MailServer);
        Assert.NotEqual("SmtpPassword123", entity.EncryptedPassword);
        Assert.Equal("SmtpPassword123", encryptionService.Decrypt(entity.EncryptedPassword));
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
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:SecretKey"] = "12345678901234567890123456789012"
            })
            .Build();

        return new EncryptionService(configuration);
    }
}
