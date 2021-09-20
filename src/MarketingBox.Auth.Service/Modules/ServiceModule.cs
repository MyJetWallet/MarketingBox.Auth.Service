using Autofac;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Messages;
using MarketingBox.Auth.Service.Messages.Users;
using MarketingBox.Auth.Service.MyNoSql;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;

namespace MarketingBox.Auth.Service.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder
                .RegisterMyServiceBusTcpClient(
                    Program.ReloadedSettings(e => e.MarketingBoxServiceBusHostPort),
                    ApplicationEnvironment.HostName, Program.LogFactory);

            var noSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));

            #region Partners

            // publisher (IPublisher<PartnerUpdated>)
            builder.RegisterMyServiceBusPublisher<UserUpdated>(serviceBusClient, Topics.UserUpdatedTopic, false);

            // publisher (IPublisher<PartnerRemoved>)
            builder.RegisterMyServiceBusPublisher<UserRemoved>(serviceBusClient, Topics.UserRemovedTopic, false);

            // register writer (IMyNoSqlServerDataWriter<PartnerNoSql>)
            builder.RegisterMyNoSqlWriter<UserNoSql>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), UserNoSql.TableName);

            #endregion
        }
    }
}
