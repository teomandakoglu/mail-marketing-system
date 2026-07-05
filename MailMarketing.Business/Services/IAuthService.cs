using MailMarketing.Business.DTOs.Auth;

namespace MailMarketing.Business.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto registerDto);

    Task<string?> LoginAsync(LoginDto loginDto);
}
