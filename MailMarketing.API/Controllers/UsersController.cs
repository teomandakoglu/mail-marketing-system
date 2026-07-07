using System.Security.Claims;
using MailMarketing.Business.DTOs.Users;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MailMarketing.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly MailMarketingDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public UsersController(MailMarketingDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users
            .AsNoTracking()
            .Where(item => item.Id == userId && item.IsActive)
            .Select(item => new UserProfileDto
            {
                Id = item.Id,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email
            })
            .SingleOrDefaultAsync();

        return user is null ? NotFound("Kullanıcı bulunamadı") : Ok(user);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto updateUserProfileDto)
    {
        var userId = GetCurrentUserId();
        var user = await _context.Users.SingleOrDefaultAsync(item => item.Id == userId && item.IsActive);

        if (user is null)
        {
            return NotFound("Kullanıcı bulunamadı");
        }

        var normalizedEmail = updateUserProfileDto.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Users.AnyAsync(item => item.Id != userId && item.Email == normalizedEmail);

        if (emailExists)
        {
            return BadRequest("Bu mail adresi başka bir kullanıcı tarafından kullanılıyor.");
        }

        user.FirstName = updateUserProfileDto.FirstName.Trim();
        user.LastName = updateUserProfileDto.LastName.Trim();
        user.Email = normalizedEmail;
        user.EncryptedPassword = _encryptionService.Encrypt(updateUserProfileDto.Password);

        await _context.SaveChangesAsync();

        return Ok("Profil güncellendi.");
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return int.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
