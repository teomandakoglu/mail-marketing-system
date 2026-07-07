using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MailMarketing.Business.DTOs.Auth;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MailMarketing.Business.Services;

public class AuthService : IAuthService
{
    private readonly MailMarketingDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly IConfiguration _configuration;

    public AuthService(
        MailMarketingDbContext context,
        IEncryptionService encryptionService,
        IConfiguration configuration)
    {
        _context = context;
        _encryptionService = encryptionService;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterDto registerDto)
    {
        var normalizedEmail = registerDto.Email.Trim().ToLowerInvariant();
        var emailExists = await _context.Users.AnyAsync(user => user.Email == normalizedEmail);

        if (emailExists)
        {
            return false;
        }

        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            Email = normalizedEmail,
            EncryptedPassword = _encryptionService.Encrypt(registerDto.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(LoginDto loginDto)
    {
        var normalizedEmail = loginDto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail && user.IsActive);

        if (user is null)
        {
            return null;
        }

        var decryptedPassword = _encryptionService.Decrypt(user.EncryptedPassword);

        return decryptedPassword == loginDto.Password ? GenerateJwtToken(user) : null;
    }

    public async Task<bool> CheckEmailAsync(CheckEmailDto checkEmailDto)
    {
        var normalizedEmail = checkEmailDto.Email.Trim().ToLowerInvariant();

        return await _context.Users.AnyAsync(user => user.Email == normalizedEmail && user.IsActive);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var normalizedEmail = resetPasswordDto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.SingleOrDefaultAsync(user => user.Email == normalizedEmail && user.IsActive);

        if (user is null)
        {
            return false;
        }

        user.EncryptedPassword = _encryptionService.Encrypt(resetPasswordDto.NewPassword);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> GenerateTokenForUserAsync(int userId)
    {
        var user = await _context.Users.SingleOrDefaultAsync(user => user.Id == userId && user.IsActive);

        return user is null ? null : GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
        {
            throw new InvalidOperationException("JWT key must be at least 32 UTF-8 bytes.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
