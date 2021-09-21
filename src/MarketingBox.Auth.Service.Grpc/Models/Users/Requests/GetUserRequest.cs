using System.Runtime.Serialization;

namespace MarketingBox.Auth.Service.Grpc.Models.Users.Requests
{
    [DataContract]
    public class GetUserRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)]
        public string ExternalUserId { get; set; }

    }
}
