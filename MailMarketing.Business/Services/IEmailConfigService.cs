using MailMarketing.Business.DTOs.EmailConfigs;

namespace MailMarketing.Business.Services;

public interface IEmailConfigService
{
    Task<EmailConfigDto?> GetByUserIdAsync(int userId);

    Task<EmailConfigDto> CreateOrUpdateAsync(int userId, CreateUpdateEmailConfigDto createUpdateEmailConfigDto);
}
