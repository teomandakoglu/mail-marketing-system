using MailMarketing.Core.Utilities.Security;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Tests;

public sealed class SecurityTests
{
    [Fact]
    public void EncryptionServiceEncryptsAndDecryptsText()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Encryption:SecretKey"] = "12345678901234567890123456789012"
            })
            .Build();

        IEncryptionService encryptionService = new EncryptionService(configuration);

        var encrypted = encryptionService.Encrypt("SecretPassword123");
        var decrypted = encryptionService.Decrypt(encrypted);

        Assert.NotEqual("SecretPassword123", encrypted);
        Assert.Equal("SecretPassword123", decrypted);
    }
}
