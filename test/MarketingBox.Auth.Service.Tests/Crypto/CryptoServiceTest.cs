using System;
using MarketingBox.Auth.Service.Crypto;
using NUnit.Framework;

namespace MarketingBox.Auth.Service.Tests.Crypto
{
    public class CryptoServiceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GenerateSalt()
        {
            var service = new CryptoService();
            var generateSalt = service.GenerateSalt();

            Assert.IsNotEmpty(generateSalt);
            Assert.IsNotNull(generateSalt);

            var saltBytes = generateSalt.HexStringToByteArray();

            Assert.AreEqual( 16, saltBytes.Length);
        }

        [Test]
        public void VerifyPassword()
        {
            var service = new CryptoService();
            var salt = "EE1F49E9EAB3A4D48ECCB36B263FD653";
            var password = "qwerty_123456";
            var hashedPassword = service.HashPassword(salt, password);

            Assert.AreEqual(hashedPassword, "B0041706BDD1D5C7D26521CC3EA72819B929744A6FEACD1C2786E5C4D9370BAE");
            var verified = service.VerifyHash(salt, password, hashedPassword);
            Assert.IsTrue(verified);
        }

        [Test]
        public void Encrypt()
        {
            var service = new CryptoService();
            var password = "PASSWORD";
            var initialKey = "EE1F49E9EAB3A4D48ECCB36B263FD653";
            var initialVector = "SecretKey";

            var encryptedHex = service.Encrypt(password, initialKey, initialVector);
            Assert.AreEqual("F043A6E61223354066A7F100865253A4", encryptedHex);
        }

        [Test]
        public void Decrypt()
        {
            var service = new CryptoService();
            var encryptedHex = "F043A6E61223354066A7F100865253A4";
            var initialKey = "EE1F49E9EAB3A4D48ECCB36B263FD653";
            var initialVector = "SecretKey";
            
            var password = service.Decrypt(encryptedHex, initialKey, initialVector);

            Assert.AreEqual("PASSWORD", password);
        }
    }
}
