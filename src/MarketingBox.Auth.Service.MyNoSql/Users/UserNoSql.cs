using MyNoSqlServer.Abstractions;

namespace MarketingBox.Auth.Service.MyNoSql.Users
{
    public class UserNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "marketingbox-authservice-users";
        public static string GeneratePartitionKey(string emailEncrypted) => $"{emailEncrypted}";
        public static string GenerateRowKey(string tenantId) =>
            $"{tenantId}";

        public string TenantId { get; set; }

        public string EmailEncrypted { get; set; }

        public string Username { get; set; }
        public string ExternalUserId { get; set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }
        

        public static UserNoSql Create(
            string tenantId,
            string emailEncrypted,
            string username,
            string externalUserId,
            string salt,
            string passwordHash) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(emailEncrypted),
                RowKey = GenerateRowKey(tenantId),
                TenantId = tenantId,
                PasswordHash = passwordHash,
                Salt = salt,
                Username = username,
                EmailEncrypted = emailEncrypted,
                ExternalUserId = externalUserId
            };
    }
}
