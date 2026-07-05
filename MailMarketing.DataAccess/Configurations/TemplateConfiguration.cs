using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MailMarketing.DataAccess.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.ToTable("Templates");

        builder.HasKey(template => template.Id);

        builder.Property(template => template.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(template => template.Content)
            .IsRequired();

        builder.Property(template => template.IsActive)
            .HasDefaultValue(true);

        builder.Property(template => template.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(template => template.MailLogs)
            .WithOne(mailLog => mailLog.Template)
            .HasForeignKey(mailLog => mailLog.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
