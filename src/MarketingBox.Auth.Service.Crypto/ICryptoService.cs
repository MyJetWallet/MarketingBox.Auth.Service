namespace MarketingBox.Auth.Service.Crypto
{
    public interface ICryptoService
    {
        string GenerateSalt();
        string HashPassword(string salt, string password);
        bool VerifyHash(string salt, string password, string hash);
        string Encrypt(string toEncrypt, string salt, string encryptionKey);
        string Decrypt(string toDecrypt, string salt, string encryptionKey);
    }
}