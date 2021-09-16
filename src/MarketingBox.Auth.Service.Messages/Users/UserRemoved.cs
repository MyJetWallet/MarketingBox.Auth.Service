using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Messages.Users
{
    [DataContract]
    public class UserRemoved
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string EmailEncrypted { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }
    }
}
