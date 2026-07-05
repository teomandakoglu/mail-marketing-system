using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class ApiConfigurationTests
{
    [Fact]
    public void ApiAppSettingsContainsDefaultConnection()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(GetApiProjectPath())
            .AddJsonFile("appsettings.json")
            .Build();

        Assert.Equal(
            "Host=localhost;Port=5432;Database=mail_pazarlama_db;Username=mailapp_user;Password=MailApp_2026_local!",
            configuration.GetConnectionString("DefaultConnection"));
    }

    [Fact]
    public void ApiProgramRegistersMailMarketingDbContext()
    {
        var programPath = Path.Combine(GetApiProjectPath(), "Program.cs");
        var programContent = File.ReadAllText(programPath);

        Assert.Contains("AddDbContext<MailMarketingDbContext>", programContent);
        Assert.Contains("GetConnectionString(\"DefaultConnection\")", programContent);
        Assert.Contains("UseNpgsql", programContent);
    }

    private static string GetApiProjectPath()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (currentDirectory is not null && !File.Exists(Path.Combine(currentDirectory.FullName, "MailMarketingSystem.sln")))
        {
            currentDirectory = currentDirectory.Parent;
        }

        Assert.NotNull(currentDirectory);

        return Path.Combine(currentDirectory.FullName, "MailMarketing.API");
    }
}
