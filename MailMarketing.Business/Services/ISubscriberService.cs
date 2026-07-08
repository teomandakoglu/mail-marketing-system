using MailMarketing.Business.DTOs.Subscribers;

namespace MailMarketing.Business.Services;

public interface ISubscriberService
{
    Task<List<SubscriberDto>> GetAllAsync(int userId);

    Task<bool> AddAsync(CreateSubscriberDto createSubscriberDto, int userId);

    Task<bool> AddToDefaultTenantAsync(CreateSubscriberDto createSubscriberDto);

    Task<bool> DeleteAsync(int id, int userId);
}
