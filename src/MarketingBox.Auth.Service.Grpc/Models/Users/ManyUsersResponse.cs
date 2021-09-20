using System.Collections.Generic;
using System.Runtime.Serialization;
using MarketingBox.Auth.Service.Grpc.Models.Common;

namespace MarketingBox.Auth.Service.Grpc.Models.Users
{
    [DataContract]
    public class ManyUsersResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<User> User { get; set; }

        [DataMember(Order = 1000)]
        public Error Error { get; set; }
    }
}