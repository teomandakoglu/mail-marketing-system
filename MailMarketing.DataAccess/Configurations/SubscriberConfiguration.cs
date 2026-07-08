using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailMarketing.DataAccess.Configurations;

public class SubscriberConfiguration : IEntityTypeConfiguration<Subscriber>
{
    public void Configure(EntityTypeBuilder<Subscriber> builder)
    {
        builder.ToTable("Subscribers");

        builder.HasKey(subscriber => subscriber.Id);

        builder.Property(subscriber => subscriber.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(subscriber => new { subscriber.UserId, subscriber.Email })
            .IsUnique();

        builder.Property(subscriber => subscriber.IsActive)
            .HasDefaultValue(true);

        builder.Property(subscriber => subscriber.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(subscriber => subscriber.User)
            .WithMany(user => user.Subscribers)
            .HasForeignKey(subscriber => subscriber.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
