namespace MailMarketing.Core.Utilities.Security;

public interface IEncryptionService
{
    string Encrypt(string text);

    string Decrypt(string cipherText);
}
