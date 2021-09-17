using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace MarketingBox.Auth.Service.Settings
{
    public class SettingsModel
    {
        [YamlProperty("MarketingBoxAuthService.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("MarketingBoxAuthService.MarketingBoxServiceBusHostPort")]
        public string MarketingBoxServiceBusHostPort { get; set; }

        [YamlProperty("MarketingBoxAuthService.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("MarketingBoxAuthService.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("MarketingBoxAuthService.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}
