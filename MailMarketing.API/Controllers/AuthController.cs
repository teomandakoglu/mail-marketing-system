using MailMarketing.Business.DTOs.Auth;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace MailMarketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!PasswordRegex().IsMatch(registerDto.Password))
        {
            return BadRequest("Parola en az 8 karakter, büyük harf, küçük harf ve rakam içermelidir.");
        }

        var result = await _authService.RegisterAsync(registerDto);

        return result ? Ok("User registered successfully.") : BadRequest("Email is already registered.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        return string.IsNullOrWhiteSpace(token) ? Unauthorized("Invalid email or password.") : Ok(new { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("check-email")]
    public async Task<IActionResult> CheckEmail(CheckEmailDto checkEmailDto)
    {
        var exists = await _authService.CheckEmailAsync(checkEmailDto);

        return exists ? Ok() : NotFound("Kullanıcı bulunamadı");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var result = await _authService.ResetPasswordAsync(resetPasswordDto);

        return result ? Ok("Parola güncellendi.") : NotFound("Kullanıcı bulunamadı");
    }

    [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$")]
    private static partial Regex PasswordRegex();
}
