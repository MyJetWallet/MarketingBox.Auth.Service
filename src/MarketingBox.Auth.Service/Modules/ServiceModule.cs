using Autofac;
using MarketingBox.Auth.Service.Crypto;
using MarketingBox.Auth.Service.Messages;
using MarketingBox.Auth.Service.Messages.Users;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Auth.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    Program.LogFactory);

            builder.Register(x => new CryptoService())
                .As<ICryptoService>()
                .SingleInstance();

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Users

            // publisher (IServiceBusPublisher<PartnerUpdated>)
            builder.RegisterMyServiceBusPublisher<UserUpdated>(serviceBusClient, Topics.UserUpdatedTopic, false);

            // publisher (IServiceBusPublisher<PartnerRemoved>)
            builder.RegisterMyServiceBusPublisher<UserRemoved>(serviceBusClient, Topics.UserRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<UserNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), UserNoSql.TableName);

            #endregion
        }
    }
}
