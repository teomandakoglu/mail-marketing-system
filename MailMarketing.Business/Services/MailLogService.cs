using MailMarketing.Business.DTOs.Reports;
using MailMarketing.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.Business.Services;

public class MailLogService : IMailLogService
{
    private readonly MailMarketingDbContext _context;

    public MailLogService(MailMarketingDbContext context)
    {
        _context = context;
    }

    public async Task<List<MailLogDto>> GetFilteredLogsAsync(int userId, ReportFilterDto filter)
    {
        var query = _context.MailLogs
            .AsNoTracking()
            .Include(mailLog => mailLog.Template)
            .Include(mailLog => mailLog.Subscriber)
            .Where(mailLog => mailLog.Template.CreatedByUserId == userId);

        if (filter.TemplateId.HasValue)
        {
            query = query.Where(mailLog => mailLog.TemplateId == filter.TemplateId.Value);
        }

        if (filter.StartDate.HasValue)
        {
            var startDateUtc = DateTime.SpecifyKind(filter.StartDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(mailLog => mailLog.SentAt >= startDateUtc);
        }

        if (filter.EndDate.HasValue)
        {
            var endDateUtc = DateTime.SpecifyKind(filter.EndDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            query = query.Where(mailLog => mailLog.SentAt <= endDateUtc);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var status = filter.Status.Trim();
            query = query.Where(mailLog => mailLog.Status == status);
        }

        return await query
            .OrderByDescending(mailLog => mailLog.SentAt)
            .Select(mailLog => new MailLogDto
            {
                Id = mailLog.Id,
                TemplateId = mailLog.TemplateId,
                TemplateTitle = mailLog.Template.Title,
                SubscriberId = mailLog.SubscriberId,
                SubscriberEmail = mailLog.Subscriber.Email,
                Status = mailLog.Status,
                ErrorMessage = mailLog.ErrorMessage,
                SentAt = mailLog.SentAt
            })
            .ToListAsync();
    }
}
