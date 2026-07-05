using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailMarketing.DataAccess.Configurations;

public class MailLogConfiguration : IEntityTypeConfiguration<MailLog>
{
    public void Configure(EntityTypeBuilder<MailLog> builder)
    {
        builder.ToTable("MailLogs");

        builder.HasKey(mailLog => mailLog.Id);

        builder.Property(mailLog => mailLog.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(mailLog => mailLog.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(mailLog => mailLog.SentAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(mailLog => mailLog.Subscriber)
            .WithMany(subscriber => subscriber.MailLogs)
            .HasForeignKey(mailLog => mailLog.SubscriberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
