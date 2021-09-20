using DotNetCoreDecorators;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Common;
using MarketingBox.Auth.Service.Grpc.Models.Users;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using MarketingBox.Auth.Service.Messages.Users;
using MarketingBox.Auth.Service.MyNoSql;
using MarketingBox.Auth.Service.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Auth.Service.MyNoSql.Users;
using MarketingBox.Auth.Service.Postgre.Entities.Users;
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
                    ExternalUserId = request.ExternalUserId,
                    EmailEncrypted = request.EmailEncrypted,
                    PasswordHash = request.PasswordHash,
                    Salt = request.Salt,
                    TenantId = request.TenantId,
                    Username = request.Username
                };

                ctx.Users.Add(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNosql(userEntity));
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
                    Username = request.Username,
                    ExternalUserId = request.ExternalUserId,
                };

                ctx.Users.Upsert(userEntity);
                await ctx.SaveChangesAsync();

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(MapToNosql(userEntity));
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

        public async Task<ManyUsersResponse> GetAsync(GetUserRequest request)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);

            try
            {
                var query = ctx.Users.AsQueryable();

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.EmailEncrypted))
                {
                    query = query.Where(x => x.EmailEncrypted == request.EmailEncrypted);
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.Username == request.Username);
                }

                if (!string.IsNullOrEmpty(request.ExternalUserId))
                {
                    query = query.Where(x => x.ExternalUserId == request.ExternalUserId);
                }

                var userEntity = await query.ToArrayAsync();

                return userEntity != null ? new ManyUsersResponse()
                {
                    User = userEntity.Select(x => MapToResponse(x).User).ToArray()
                } : new ManyUsersResponse();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during user get. {@context}", request);

                return new ManyUsersResponse()
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
                var userEntity = await ctx.Users.FirstOrDefaultAsync(x =>
                    x.TenantId == request.TenantId &&
                    x.ExternalUserId == request.ExternalUserId);

                if (userEntity == null)
                    return new UserResponse();

                await _myNoSqlServerDataWriter.DeleteAsync(UserNoSql.GeneratePartitionKey(userEntity.EmailEncrypted), 
                    UserNoSql.GenerateRowKey(userEntity.EmailEncrypted));

                await _publisherUserRemoved.PublishAsync(new UserRemoved()
                {
                    Username = userEntity.Username,
                    EmailEncrypted = userEntity.EmailEncrypted,
                    TenantId = userEntity.TenantId
                });

                await ctx.Users
                    .Where(x =>
                        x.TenantId == request.TenantId &&
                        x.ExternalUserId == request.ExternalUserId).DeleteAsync();

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
                    TenantId = userEntity.TenantId,
                    ExternalUserId = userEntity.ExternalUserId,
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
                TenantId = userEntity.TenantId,
                ExternalUserId = userEntity.ExternalUserId,
            };
        }

        private UserNoSql MapToNosql(UserEntity userEntity)
        {
            return UserNoSql.Create(
                userEntity.TenantId,
                userEntity.EmailEncrypted,
                userEntity.Username,
                userEntity.ExternalUserId,
                userEntity.Salt,
                userEntity.PasswordHash);
        }
    }
}
