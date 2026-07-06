using MailMarketing.Business.DTOs.Subscribers;

namespace MailMarketing.Business.Services;

public interface ISubscriberService
{
    Task<List<SubscriberDto>> GetAllAsync();

    Task<bool> AddAsync(CreateSubscriberDto createSubscriberDto);

    Task<bool> DeleteAsync(int id);
}
