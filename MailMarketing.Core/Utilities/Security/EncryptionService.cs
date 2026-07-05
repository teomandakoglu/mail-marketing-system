using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MailMarketing.Core.Utilities.Security;

public class EncryptionService : IEncryptionService
{
    private const int AesKeyLength = 32;
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var secretKey = configuration["Encryption:SecretKey"];

        if (string.IsNullOrWhiteSpace(secretKey) || Encoding.UTF8.GetByteCount(secretKey) != AesKeyLength)
        {
            throw new InvalidOperationException("Encryption secret key must be exactly 32 UTF-8 bytes.");
        }

        _key = Encoding.UTF8.GetBytes(secretKey);
    }

    public string Encrypt(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(text);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var resultBytes = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, resultBytes, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, resultBytes, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(resultBytes);
    }

    public string Decrypt(string cipherText)
    {
        ArgumentNullException.ThrowIfNull(cipherText);

        var fullCipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;

        var iv = new byte[aes.BlockSize / 8];
        var cipherBytes = new byte[fullCipherBytes.Length - iv.Length];

        Buffer.BlockCopy(fullCipherBytes, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipherBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}
