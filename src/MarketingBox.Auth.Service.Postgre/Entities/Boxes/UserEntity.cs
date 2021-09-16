namespace MarketingBox.Auth.Service.Postgre.Entities.Boxes
{
    public class UserEntity
    {
        public string TenantId { get; set; }

        public string EmailEncrypted { get; set; }

        public string Username { get; set; }

        public string Salt { get; set; }

        public string PasswordHash { get; set; }
        
    }
}
