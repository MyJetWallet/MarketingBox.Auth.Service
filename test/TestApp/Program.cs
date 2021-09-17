using System;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Client;
using MarketingBox.Auth.Service.Crypto;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using ProtoBuf.Grpc.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new AuthServiceClientFactory("http://localhost:12347");
            var client = factory.GetUserService();

            var passwordHAshingService = new CryptoService();
            var salt = passwordHAshingService.GenerateSalt();
            var encryptionSalt = "E89E45242FBC719D83EDCFFAED90C640";
            var password = "qwerty_123456";
            var passwordHash = passwordHAshingService.HashPassword(salt, password);
            var emailEncrypted = passwordHAshingService.Encrypt("some-email@gmail.com", encryptionSalt, "SecretKey");

            var resp = await  client.CreateAsync(new CreateUserRequest()
            {
                ExternalUserId = "GeneralManager",
                EmailEncrypted = emailEncrypted,
                PasswordHash = passwordHash,
                Salt = salt,
                TenantId = "test-tenant",
                Username = "SomeUser"
            });

            var emailEncrypted2 = passwordHAshingService.Encrypt("some-email2@gmail.com", encryptionSalt, "SecretKey");

            var updResponse = await client.UpdateAsync(new UpdateUserRequest()
            {
                ExternalUserId = "GeneralManager",
                EmailEncrypted = emailEncrypted2,
                PasswordHash = passwordHash,
                Salt = salt,
                TenantId = "test-tenant",
                Username = "SomeUser"
            });

            var get1 = await client.GetAsync(new GetUserRequest()
            {
                ExternalUserId = "GeneralManager",
                TenantId = "test-tenant",
            });

            var get2 = await client.GetAsync(new GetUserRequest()
            {
                EmailEncrypted = emailEncrypted2,
                TenantId = "test-tenant",
            });

            var get3 = await client.GetAsync(new GetUserRequest()
            {
                TenantId = "test-tenant",
                Username = "SomeUser"
            });

            var del = await client.DeleteAsync(new DeleteUserRequest()
            {
                TenantId = "test-tenant",
                ExternalUserId = "GeneralManager"
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
