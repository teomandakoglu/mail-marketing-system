using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailMarketing.DataAccess.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.EncryptedPassword)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(user => user.IsActive)
            .HasDefaultValue(true);

        builder.Property(user => user.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(user => user.Templates)
            .WithOne(template => template.User)
            .HasForeignKey(template => template.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(user => user.EmailConfigs)
            .WithOne(emailConfig => emailConfig.User)
            .HasForeignKey(emailConfig => emailConfig.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
