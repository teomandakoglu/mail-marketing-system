using MailMarketing.Business.DTOs.Auth;
using MailMarketing.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace MailMarketing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        return result ? Ok("User registered successfully.") : BadRequest("Email is already registered.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        return string.IsNullOrWhiteSpace(token) ? Unauthorized("Invalid email or password.") : Ok(new { Token = token });
    }
}
