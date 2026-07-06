using System.Security.Claims;
using MailMarketing.Business.DTOs.Reports;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IMailLogService _mailLogService;

    public ReportsController(IMailLogService mailLogService)
    {
        _mailLogService = mailLogService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ReportFilterDto filter)
    {
        var userId = GetCurrentUserId();
        var logs = await _mailLogService.GetFilteredLogsAsync(userId, filter);

        return Ok(logs);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
