using System.Security.Claims;
using MailMarketing.Business.DTOs.Templates;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var templates = await _templateService.GetAllAsync(GetCurrentUserId());

        return Ok(templates);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var template = await _templateService.GetByIdAsync(id, GetCurrentUserId());

        return template is null ? NotFound() : Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Add(CreateTemplateDto createTemplateDto)
    {
        var userId = GetCurrentUserId();
        var template = await _templateService.AddAsync(createTemplateDto, userId);

        return Ok(template);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTemplateDto updateTemplateDto)
    {
        var result = await _templateService.UpdateAsync(id, updateTemplateDto, GetCurrentUserId());

        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _templateService.DeleteAsync(id, GetCurrentUserId());

        return result ? NoContent() : BadRequest("Template could not be deleted.");
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
