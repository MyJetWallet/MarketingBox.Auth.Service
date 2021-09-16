using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Messages.Users
{
    [DataContract]
    public class UserUpdated
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string EmailEncrypted { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)]
        public string Salt { get; set; }

        [DataMember(Order = 5)]
        public string PasswordHash { get; set; }
        
    }
}
