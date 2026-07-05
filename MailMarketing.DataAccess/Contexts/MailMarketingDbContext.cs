using System.Reflection;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.DataAccess.Contexts;

public class MailMarketingDbContext : DbContext
{
    public MailMarketingDbContext(DbContextOptions<MailMarketingDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<EmailConfig> EmailConfigs => Set<EmailConfig>();

    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    public DbSet<Template> Templates => Set<Template>();

    public DbSet<MailLog> MailLogs => Set<MailLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
