using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class DashboardService : IDashboardService
{
    private readonly MailMarketingDbContext _dbContext;

    public DashboardService(MailMarketingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
    {
        return new DashboardStatsDto
        {
            TotalSubscribers = await _dbContext.Subscribers.CountAsync(subscriber => subscriber.UserId == userId),
            TotalTemplates = await _dbContext.Templates.CountAsync(template => template.CreatedByUserId == userId),
            TotalSentMails = await _dbContext.MailLogs.CountAsync(mailLog => mailLog.Template.CreatedByUserId == userId)
        };
    }
}
