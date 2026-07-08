using MailMarketing.Business.DTOs.Auth;
using MailMarketing.Business.Services;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterEncryptsPasswordAndLoginReturnsToken()
    {
        await using var context = CreateContext();
        var encryptionService = CreateEncryptionService();
        IAuthService authService = new AuthService(context, encryptionService, CreateConfiguration());

        var registerResult = await authService.RegisterAsync(new RegisterDto
        {
            FirstName = "Teoman",
            LastName = "Tester",
            Email = "teoman@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        });

        var user = await context.Users.SingleAsync();
        var token = await authService.LoginAsync(new LoginDto
        {
            Email = "teoman@example.com",
            Password = "Password123"
        });

        Assert.True(registerResult);
        Assert.NotEqual("Password123", user.EncryptedPassword);
        Assert.Equal("Password123", encryptionService.Decrypt(user.EncryptedPassword));
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task RegisterRejectsMismatchedConfirmPassword()
    {
        await using var context = CreateContext();
        IAuthService authService = new AuthService(context, CreateEncryptionService(), CreateConfiguration());

        var result = await authService.RegisterAsync(new RegisterDto
        {
            FirstName = "Mismatch",
            LastName = "User",
            Email = "mismatch@example.com",
            Password = "Password123",
            ConfirmPassword = "Password124"
        });

        Assert.False(result);
        Assert.Empty(context.Users);
    }

    [Fact]
    public async Task LoginDistinguishesMissingUserFromWrongPassword()
    {
        await using var context = CreateContext();
        IAuthService authService = new AuthService(context, CreateEncryptionService(), CreateConfiguration());
        await authService.RegisterAsync(new RegisterDto
        {
            FirstName = "Login",
            LastName = "User",
            Email = "login@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123"
        });

        var missingUser = await authService.LoginDetailedAsync(new LoginDto
        {
            Email = "missing@example.com",
            Password = "Password123"
        });
        var wrongPassword = await authService.LoginDetailedAsync(new LoginDto
        {
            Email = "login@example.com",
            Password = "WrongPassword123"
        });
        var success = await authService.LoginDetailedAsync(new LoginDto
        {
            Email = "login@example.com",
            Password = "Password123"
        });

        Assert.Equal(LoginFailureReason.UserNotFound, missingUser.FailureReason);
        Assert.Equal(LoginFailureReason.WrongPassword, wrongPassword.FailureReason);
        Assert.False(string.IsNullOrWhiteSpace(success.Token));
        Assert.Null(success.FailureReason);
    }

    private static MailMarketingDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailMarketingDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MailMarketingDbContext(options);
    }

    private static IEncryptionService CreateEncryptionService()
    {
        return new EncryptionService(CreateConfiguration());
    }

    private static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:SecretKey"] = "12345678901234567890123456789012",
                ["Jwt:Key"] = "JwtSecretKeyForMailMarketingSystem12345"
            })
            .Build();
    }
}
