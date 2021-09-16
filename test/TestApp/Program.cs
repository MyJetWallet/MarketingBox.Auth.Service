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


            var factory = new AuthServiceClientFactory("http://localhost:5001");
            var client = factory.GetUserService();

            var passwordHAshingService = new CryptoService();
            var salt = passwordHAshingService.GenerateSalt();
            var password = "qwerty_123456";
            var passwordHash = passwordHAshingService.HashPassword(salt, password);

            var resp = await  client.CreateAsync(new CreateUserRequest()
            {
                EmailEncrypted = "some-email@gmail.com",
                PasswordHash = passwordHash,
                Salt = salt,
                TenantId = "test-tenant",
                Username = "SomeUser"
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
