using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailMarketing.DataAccess.Configurations;

public class EmailConfigConfiguration : IEntityTypeConfiguration<EmailConfig>
{
    public void Configure(EntityTypeBuilder<EmailConfig> builder)
    {
        builder.ToTable("EmailConfigs");

        builder.HasKey(emailConfig => emailConfig.Id);

        builder.Property(emailConfig => emailConfig.MailServer)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(emailConfig => emailConfig.SmtpPort)
            .IsRequired();

        builder.Property(emailConfig => emailConfig.UseSsl)
            .IsRequired();

        builder.Property(emailConfig => emailConfig.EmailAddress)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(emailConfig => emailConfig.EncryptedPassword)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(emailConfig => emailConfig.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
