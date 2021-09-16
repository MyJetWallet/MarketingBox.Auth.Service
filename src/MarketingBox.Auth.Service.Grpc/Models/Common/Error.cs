using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketingBox.Auth.Service.Grpc.Models.Common
{
    [DataContract]
    public class Error
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public ErrorType ErrorType { get; set; }

    }
}
