using MailMarketing.Core.Utilities.Entities;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MailMarketing.Tests;

public sealed class DataModelTests
{
    [Fact]
    public void ConcreteEntitiesImplementMarkerInterface()
    {
        Assert.True(typeof(IEntity).IsAssignableFrom(typeof(User)));
        Assert.True(typeof(IEntity).IsAssignableFrom(typeof(EmailConfig)));
        Assert.True(typeof(IEntity).IsAssignableFrom(typeof(Subscriber)));
        Assert.True(typeof(IEntity).IsAssignableFrom(typeof(Template)));
        Assert.True(typeof(IEntity).IsAssignableFrom(typeof(MailLog)));
    }

    [Fact]
    public void EntityDefaultsMatchDomainRequirements()
    {
        var user = new User();
        var subscriber = new Subscriber();
        var template = new Template();

        Assert.True(user.IsActive);
        Assert.True(subscriber.IsActive);
        Assert.True(template.IsActive);
        Assert.True(user.CreatedAt <= DateTime.UtcNow);
        Assert.True(subscriber.CreatedAt <= DateTime.UtcNow);
        Assert.True(template.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void DbContextExposesRequiredSets()
    {
        using var context = CreateContext();

        Assert.NotNull(context.Users);
        Assert.NotNull(context.EmailConfigs);
        Assert.NotNull(context.Subscribers);
        Assert.NotNull(context.Templates);
        Assert.NotNull(context.MailLogs);
    }

    [Fact]
    public void UserAndSubscriberEmailIndexesAreUnique()
    {
        using var context = CreateContext();

        AssertUniqueIndex<User>(context, nameof(User.Email));
        AssertUniqueIndex<Subscriber>(context, nameof(Subscriber.Email));
    }

    [Fact]
    public void RelationshipsUseExpectedDeleteBehaviors()
    {
        using var context = CreateContext();

        AssertForeignKey<User, EmailConfig>(context, nameof(EmailConfig.UserId), DeleteBehavior.Cascade);
        AssertForeignKey<User, Subscriber>(context, nameof(Subscriber.UserId), DeleteBehavior.Cascade);
        AssertForeignKey<User, Template>(context, nameof(Template.CreatedByUserId), DeleteBehavior.Restrict);
        AssertForeignKey<Template, MailLog>(context, nameof(MailLog.TemplateId), DeleteBehavior.Restrict);
        AssertForeignKey<Subscriber, MailLog>(context, nameof(MailLog.SubscriberId), DeleteBehavior.Restrict);
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=mail_pazarlama_db;Username=mailapp_user;Password=MailApp_2026_local!")
            .Options;

        return new MailMarketingDbContext(options);
    }

    private static void AssertUniqueIndex<TEntity>(DbContext context, string propertyName)
        where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        Assert.NotNull(entityType);

        var index = entityType.GetIndexes()
            .SingleOrDefault(i => i.Properties.Any(p => p.Name == propertyName));

        Assert.NotNull(index);
        Assert.True(index.IsUnique);
    }

    private static void AssertForeignKey<TPrincipal, TDependent>(
        DbContext context,
        string propertyName,
        DeleteBehavior expectedDeleteBehavior)
        where TPrincipal : class
        where TDependent : class
    {
        var dependentType = context.Model.FindEntityType(typeof(TDependent));
        Assert.NotNull(dependentType);

        var foreignKey = dependentType.GetForeignKeys()
            .SingleOrDefault(fk =>
                fk.PrincipalEntityType.ClrType == typeof(TPrincipal) &&
                fk.Properties.Any(p => p.Name == propertyName));

        Assert.NotNull(foreignKey);
        Assert.Equal(expectedDeleteBehavior, foreignKey.DeleteBehavior);
    }
}
