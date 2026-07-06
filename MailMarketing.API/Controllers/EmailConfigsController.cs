using System.Security.Claims;
using MailMarketing.Business.DTOs.EmailConfigs;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmailConfigsController : ControllerBase
{
    private readonly IEmailConfigService _emailConfigService;

    public EmailConfigsController(IEmailConfigService emailConfigService)
    {
        _emailConfigService = emailConfigService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetCurrentUserId();
        var emailConfig = await _emailConfigService.GetByUserIdAsync(userId);

        return emailConfig is null ? NotFound() : Ok(emailConfig);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdate(CreateUpdateEmailConfigDto createUpdateEmailConfigDto)
    {
        var userId = GetCurrentUserId();
        var emailConfig = await _emailConfigService.CreateOrUpdateAsync(userId, createUpdateEmailConfigDto);

        return Ok(emailConfig);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
