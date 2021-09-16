using System.Runtime.Serialization;
using MarketingBox.Auth.Service.Grpc.Models.Common;

namespace MarketingBox.Auth.Service.Grpc.Models.Users
{
    [DataContract]
    public class UserResponse
    {
        [DataMember(Order = 1)]
        public User User { get; set; }

        [DataMember(Order = 1000)]
        public Error Error { get; set; }
    }
}