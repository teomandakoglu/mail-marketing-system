using MailMarketing.Entities.Dtos;

namespace MailMarketing.Business.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(int userId);
}
