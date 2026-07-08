using System.Security.Claims;
using MailMarketing.Business.DTOs.Subscribers;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubscribersController : ControllerBase
{
    private readonly ISubscriberService _subscriberService;

    public SubscribersController(ISubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subscribers = await _subscriberService.GetAllAsync(GetCurrentUserId());

        return Ok(subscribers);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateSubscriberDto createSubscriberDto)
    {
        var result = await _subscriberService.AddAsync(createSubscriberDto, GetCurrentUserId());

        return result ? Ok("Subscriber created successfully.") : BadRequest("Subscriber email already exists.");
    }

    [AllowAnonymous]
    [HttpPost("public")]
    public async Task<IActionResult> PublicSubscribe(CreateSubscriberDto createSubscriberDto)
    {
        var result = await _subscriberService.AddToDefaultTenantAsync(createSubscriberDto);

        return result ? Ok("Kaydedildi.") : BadRequest("Bu mail adresi zaten kayıtlı.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriberService.DeleteAsync(id, GetCurrentUserId());

        return result ? NoContent() : BadRequest("Subscriber could not be deleted.");
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
