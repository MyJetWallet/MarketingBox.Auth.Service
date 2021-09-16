using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models.Users.Requests
{
    [DataContract]
    public class DeleteUserRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string EmailEncrypted { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

    }
}
