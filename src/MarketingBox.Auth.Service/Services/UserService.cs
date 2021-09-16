using DotNetCoreDecorators;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Common;
using MarketingBox.Auth.Service.Grpc.Models.Users;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using MarketingBox.Auth.Service.Messages.Users;
using MarketingBox.Auth.Service.MyNoSql;
using MarketingBox.Auth.Service.Postgre;
using MarketingBox.Auth.Service.Postgre.Entities.Boxes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace MarketingBox.Auth.Service.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IMyNoSqlServerDataWriter<UserNoSql> _myNoSqlServerDataWriter;
        private readonly IPublisher<UserUpdated> _publisherUserUpdated;
        private readonly IPublisher<UserRemoved> _publisherUserRemoved;

        public UserService(ILogger<UserService> logger,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IMyNoSqlServerDataWriter<UserNoSql> myNoSqlServerDataWriter,
            IPublisher<UserUpdated> publisherUserUpdated,
            IPublisher<UserRemoved> publisherUserRemoved)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _publisherUserUpdated = publisherUserUpdated;
            _publisherUserRemoved = publisherUserRemoved;
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            _logger.LogInformation("Creating new User {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var userEntity = new UserEntity()
                {
                    EmailEncrypted = request.EmailEncrypted,
                    PasswordHash = request.PasswordHash,
                    Salt = request.Salt,
                    TenantId = request.TenantId,
                    Username = request.Username
                };

                ctx.Users.Add(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertAsync(MapToNosql(userEntity));
                _logger.LogInformation("Created new User in NoSQL {@context}", request);

                await _publisherUserUpdated.PublishAsync(MapToMessage(userEntity));
                _logger.LogInformation("Sent event Created new User {@context}", request);

                return MapToResponse(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user creation. {@context}", request);

                return new UserResponse()
                {
                    Error = new Error() { ErrorType = ErrorType.Unknown, Message = e.Message }
                };
            }
        }

        public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
        {
            _logger.LogInformation("Updating User {@context}", request);
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {

                var userEntity = new UserEntity()
                {
                    EmailEncrypted = request.EmailEncrypted,
                    PasswordHash = request.PasswordHash,
                    Salt = request.Salt,
                    TenantId = request.TenantId,
                    Username = request.Username
                };

                ctx.Users.Upsert(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertAsync(MapToNosql(userEntity));
                _logger.LogInformation("Updated User in NoSQL {@context}", request);

                await _publisherUserUpdated.PublishAsync(MapToMessage(userEntity));
                _logger.LogInformation("Sent event Updated User {@context}", request);

                return MapToResponse(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user update. {@context}", request);

                return new UserResponse()
                {
                    Error = new Error() { ErrorType = ErrorType.Unknown, Message = e.Message }
                };
            }
        }

        public async Task<UserResponse> GetAsync(GetUserRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var userEntity = await ctx.Users
                    .FirstOrDefaultAsync(x => 
                        x.TenantId == request.TenantId && 
                        (x.EmailEncrypted == request.EmailEncrypted || x.Username == request.Username));

                return MapToResponse(userEntity);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@context}", request);

                return new UserResponse()
                {
                    Error = new Error() { ErrorType = ErrorType.Unknown, Message = e.Message }
                };
            }
        }

        public async Task<UserResponse> DeleteAsync(DeleteUserRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                await ctx.Users
                    .Where(x =>
                        x.TenantId == request.TenantId &&
                        (x.EmailEncrypted == request.EmailEncrypted || x.Username == request.Username)).DeleteAsync();

                await _myNoSqlServerDataWriter.DeleteAsync(UserNoSql.GeneratePartitionKey(request.TenantId), 
                    UserNoSql.GenerateRowKey(request.EmailEncrypted));

                await _publisherUserRemoved.PublishAsync(new UserRemoved()
                {
                    Username = request.Username,
                    EmailEncrypted = request.EmailEncrypted,
                    TenantId = request.TenantId
                });

                return new UserResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@context}", request);

                return new UserResponse()
                {
                    Error = new Error() { ErrorType = ErrorType.Unknown, Message = e.Message }
                };
            }
        }

        private UserResponse MapToResponse(UserEntity userEntity)
        {
            return new UserResponse()
            {
                User = new User()
                {
                    Username = userEntity.Username,
                    Salt = userEntity.Salt,
                    PasswordHash = userEntity.PasswordHash,
                    EmailEncrypted = userEntity.EmailEncrypted,
                    TenantId = userEntity.TenantId
                }
            };
        }

        private UserUpdated MapToMessage(UserEntity userEntity)
        {
            return new UserUpdated()
            {
                Salt = userEntity.Salt,
                PasswordHash = userEntity.PasswordHash,
                Username = userEntity.Username,
                EmailEncrypted = userEntity.EmailEncrypted,
                TenantId = userEntity.TenantId
            };
        }

        private UserNoSql MapToNosql(UserEntity userEntity)
        {
            return UserNoSql.Create(
                userEntity.TenantId,
                userEntity.EmailEncrypted,
                userEntity.Username,
                userEntity.Salt,
                userEntity.PasswordHash);
        }
    }
}
