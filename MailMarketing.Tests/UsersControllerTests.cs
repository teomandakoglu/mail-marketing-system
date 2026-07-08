using System.Security.Claims;
using MailMarketing.API.Controllers;
using MailMarketing.Business.DTOs.Users;
using MailMarketing.Business.Services;
using MailMarketing.Core.Utilities.Security;
using MailMarketing.DataAccess.Contexts;
using MailMarketing.Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class UsersControllerTests
{
    [Fact]
    public async Task GetAllReturnsUsersWithActiveStateAndCreatedAt()
    {
        await using var context = CreateContext();
        await SeedUsersAsync(context);
        var controller = CreateController(context, currentUserId: 1);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var users = Assert.IsAssignableFrom<IReadOnlyList<UserListDto>>(ok.Value);
        Assert.Equal(2, users.Count);
        Assert.Equal("Active", users[0].FirstName);
        Assert.Equal("User", users[0].LastName);
        Assert.Equal("active@example.com", users[0].Email);
        Assert.True(users[0].IsActive);
        Assert.Equal(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc), users[0].CreatedAt);
    }

    [Fact]
    public async Task DeactivateMarksSelectedUserInactive()
    {
        await using var context = CreateContext();
        await SeedUsersAsync(context);
        var controller = CreateController(context, currentUserId: 1);

        var result = await controller.Deactivate(2);

        Assert.IsType<NoContentResult>(result);
        var user = await context.Users.SingleAsync(item => item.Id == 2);
        Assert.False(user.IsActive);
    }

    [Fact]
    public async Task DeactivateRejectsCurrentUser()
    {
        await using var context = CreateContext();
        await SeedUsersAsync(context);
        var controller = CreateController(context, currentUserId: 1);

        var result = await controller.Deactivate(1);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Kendi kullanıcınızı pasife alamazsınız.", badRequest.Value);
    }

    private static UsersController CreateController(MailMarketingDbContext context, int currentUserId)
    {
        return new UsersController(context, CreateEncryptionService(), CreateAuthService(context))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, currentUserId.ToString())
                    }, "TestAuth"))
                }
            }
        };
    }

    private static async Task SeedUsersAsync(MailMarketingDbContext context)
    {
        context.Users.AddRange(
            new User
            {
                Id = 1,
                FirstName = "Active",
                LastName = "User",
                Email = "active@example.com",
                EncryptedPassword = "encrypted",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                FirstName = "Target",
                LastName = "User",
                Email = "target@example.com",
                EncryptedPassword = "encrypted",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 2, 10, 0, 0, DateTimeKind.Utc)
            });
        await context.SaveChangesAsync();
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

    private static IAuthService CreateAuthService(MailMarketingDbContext context)
    {
        return new AuthService(context, CreateEncryptionService(), CreateConfiguration());
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
