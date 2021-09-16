using System;
using MarketingBox.Auth.Service.Crypto;
using NUnit.Framework;

namespace MarketingBox.Auth.Service.Tests.Crypto
{
    public class EncryptorDecryptorTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Encrypt()
        {
            var password = "PASSWORD";
            var initialKey = "EE1F49E9EAB3A4D48ECCB36B263FD653".HexStringToByteArray();
            var initialVector = new byte[System.Text.Encoding.UTF8.GetByteCount("SecretKey")];
            System.Text.Encoding.UTF8.GetBytes("SecretKey", initialVector);

            var encryptedHex = password.Encrypt(initialKey, initialVector);
            Assert.AreEqual("F043A6E61223354066A7F100865253A4", encryptedHex);
        }

        [Test]
        public void Decrypt()
        {
            var encryptedHex = "F043A6E61223354066A7F100865253A4";
            var initialKey = "EE1F49E9EAB3A4D48ECCB36B263FD653".HexStringToByteArray();
            var initialVector = new byte[System.Text.Encoding.UTF8.GetByteCount("SecretKey")];
            System.Text.Encoding.UTF8.GetBytes("SecretKey", initialVector);
            
            var password = encryptedHex.Decrypt(initialKey, initialVector);

            Assert.AreEqual("PASSWORD", password);
        }
    }
}
