using MailMarketing.Business.DTOs.Auth;

namespace MailMarketing.Business.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto registerDto);

    Task<string?> LoginAsync(LoginDto loginDto);

    Task<LoginResult> LoginDetailedAsync(LoginDto loginDto);

    Task<bool> CheckEmailAsync(CheckEmailDto checkEmailDto);

    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

    Task<string?> GenerateTokenForUserAsync(int userId);
}
