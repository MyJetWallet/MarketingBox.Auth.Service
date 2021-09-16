using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.Grpc.Models;
using MarketingBox.Auth.Service.Grpc.Models.Users;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;

namespace MarketingBox.Auth.Service.Grpc
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Task<UserResponse> CreateAsync(CreateUserRequest request);
        
        [OperationContract]
        Task<UserResponse> UpdateAsync(UpdateUserRequest request);
        
        [OperationContract]
        Task<UserResponse> GetAsync(GetUserRequest request);
        
        [OperationContract]
        Task<UserResponse> DeleteAsync(DeleteUserRequest request);
    }
}
