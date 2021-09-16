using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace MarketingBox.Auth.Service.Crypto
{
    public class CryptoService : ICryptoService
    {
        public string GenerateSalt()
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt.ToHexString();
        }

        public string HashPassword(string salt, string password)
        {
            var saltBytes = salt.HexStringToByteArray();
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32).ToHexString();

            return hashed;
        }

        public bool VerifyHash(string salt, string password, string hash)
        {
            var curHash = HashPassword(salt, password);

            return curHash.Equals(hash);
        }

        public string Encrypt(string toEncrypt, string salt, string encryptionKey)
        {
            var initialKey = salt.HexStringToByteArray();
            var initialVector = new byte[System.Text.Encoding.UTF8.GetByteCount(encryptionKey)];
            System.Text.Encoding.UTF8.GetBytes(encryptionKey, initialVector);

            var encryptedHex = toEncrypt.Encrypt(initialKey, initialVector);
            return encryptedHex;
        }

        public string Decrypt(string toDecrypt, string salt, string encryptionKey)
        {
            var initialKey = salt.HexStringToByteArray();
            var initialVector = new byte[System.Text.Encoding.UTF8.GetByteCount(encryptionKey)];
            System.Text.Encoding.UTF8.GetBytes(encryptionKey, initialVector);

            var initialValue = toDecrypt.Decrypt(initialKey, initialVector);
            return initialValue;
        }
    }
}
