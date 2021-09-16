using Autofac;
using MarketingBox.Auth.Service.Crypto;
using MarketingBox.Auth.Service.Grpc;

// ReSharper disable UnusedMember.Global

namespace MarketingBox.Auth.Service.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAuthServiceClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AuthServiceClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetUserService()).As<IUserService>().SingleInstance();
            builder.RegisterInstance(new CryptoService()).As<ICryptoService>().SingleInstance();
        }
    }
}
