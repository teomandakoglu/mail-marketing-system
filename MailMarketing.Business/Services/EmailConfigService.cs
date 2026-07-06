using MailMarketing.Business.DTOs.EmailConfigs;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class EmailConfigService : IEmailConfigService
{
    private readonly MailMarketingDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public EmailConfigService(MailMarketingDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    public async Task<EmailConfigDto?> GetByUserIdAsync(int userId)
    {
        return await _context.EmailConfigs
            .AsNoTracking()
            .Where(config => config.UserId == userId)
            .Select(config => MapToDto(config))
            .SingleOrDefaultAsync();
    }

    public async Task<EmailConfigDto> CreateOrUpdateAsync(int userId, CreateUpdateEmailConfigDto createUpdateEmailConfigDto)
    {
        var emailConfig = await _context.EmailConfigs.SingleOrDefaultAsync(config => config.UserId == userId);
        var encryptedPassword = _encryptionService.Encrypt(createUpdateEmailConfigDto.Password);

        if (emailConfig is null)
        {
            emailConfig = new EmailConfig
            {
                UserId = userId,
                MailServer = createUpdateEmailConfigDto.MailServer.Trim(),
                SmtpPort = createUpdateEmailConfigDto.SmtpPort,
                UseSsl = createUpdateEmailConfigDto.UseSsl,
                EmailAddress = createUpdateEmailConfigDto.EmailAddress.Trim().ToLowerInvariant(),
                EncryptedPassword = encryptedPassword,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.EmailConfigs.AddAsync(emailConfig);
        }
        else
        {
            emailConfig.MailServer = createUpdateEmailConfigDto.MailServer.Trim();
            emailConfig.SmtpPort = createUpdateEmailConfigDto.SmtpPort;
            emailConfig.UseSsl = createUpdateEmailConfigDto.UseSsl;
            emailConfig.EmailAddress = createUpdateEmailConfigDto.EmailAddress.Trim().ToLowerInvariant();
            emailConfig.EncryptedPassword = encryptedPassword;
            emailConfig.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return MapToDto(emailConfig);
    }

    private static EmailConfigDto MapToDto(EmailConfig emailConfig)
    {
        return new EmailConfigDto
        {
            Id = emailConfig.Id,
            UserId = emailConfig.UserId,
            MailServer = emailConfig.MailServer,
            SmtpPort = emailConfig.SmtpPort,
            UseSsl = emailConfig.UseSsl,
            EmailAddress = emailConfig.EmailAddress,
            UpdatedAt = emailConfig.UpdatedAt
        };
    }
}
