using MyJetWallet.Sdk.Postgres;

namespace MarketingBox.Auth.Service.Postgre.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
    {
        public ContextFactory() : base(options => new DatabaseContext(options))
        {

        }
    }
}