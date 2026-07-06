using MailMarketing.Business.DTOs.Reports;

namespace MailMarketing.Business.Services;

public interface IMailLogService
{
    Task<List<MailLogDto>> GetFilteredLogsAsync(int userId, ReportFilterDto filter);
}
