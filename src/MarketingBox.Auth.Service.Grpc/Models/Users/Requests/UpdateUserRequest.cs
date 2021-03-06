using System.Runtime.Serialization;
using Destructurama.Attributed;

namespace MarketingBox.Auth.Service.Grpc.Models.Users.Requests
{
    [DataContract]
    public class UpdateUserRequest
    {
        [DataMember(Order = 1)]
        public string TenantId { get; set; }

        [DataMember(Order = 2)]
        [LogMasked(PreserveLength = false)]
        public string Email { get; set; }

        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)]
        [LogMasked(PreserveLength = false)]
        public string Password { get; set; }

        [DataMember(Order = 5)]
        public string ExternalUserId { get; set; }

    }
}
