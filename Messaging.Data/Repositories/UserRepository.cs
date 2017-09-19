using System;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Data.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private const string GlobalPartitionKey = "Global";
        private const string UserRowKeyPrefix = "U;";

        private readonly CloudTable _table;

        public UserRepository(TableClientFactory clientFactory)
        {
            _table = clientFactory.GetTableClient()
                .GetTableReference("DataTable");
        }

        public async Task<bool> IsValidUser(string email)
        {
            var user = await GetUser(email);
            return user != null;
        }

        public async Task<User> Get(Guid appId, string email)
        {
            var user = await GetUser(email);
            if (user == null)
                return null;

            var userMetadata = await GetUserMetadata(appId, user.RowKey);
            if (userMetadata == null)
                return null;

            return new User
            {
                Email = user.PartitionKey,
                CustomData = userMetadata.CustomData
            };
        }

        private async Task<UserEntity> GetUser(string email)
        {
            var userResult = await _table.ExecuteAsync(TableOperation.Retrieve<UserEntity>(GlobalPartitionKey, GetUserRowKey(email)));
            return (UserEntity)userResult.Result;
        }

        private async Task<UserMetadataEntity> GetUserMetadata(Guid appId, string rowKey)
        {
            var userMetadataResult = await _table.ExecuteAsync(TableOperation.Retrieve<UserMetadataEntity>(appId.ToString(), rowKey));
            return (UserMetadataEntity)userMetadataResult.Result;
        }

        private string GetUserRowKey(string email)
        {
            return UserRowKeyPrefix + email;
        }

        public async Task<User> Upsert(Guid appId, User user)
        {
            var userEntity = await GetUser(user.Email);
            if (userEntity == null)
            {
                userEntity = new UserEntity
                {
                    PartitionKey = GlobalPartitionKey,
                    RowKey = GetUserRowKey(user.Email),
                };
                var userResult = await _table.ExecuteAsync(TableOperation.InsertOrReplace(userEntity));
                userEntity = (UserEntity)userResult.Result;
            }

            var userMetadataEntity = new UserMetadataEntity
            {
                PartitionKey = appId.ToString(),
                RowKey = userEntity.RowKey,
                CustomData = user.CustomData
            };
            var result = await _table.ExecuteAsync(TableOperation.InsertOrReplace(userMetadataEntity));
            userMetadataEntity = (UserMetadataEntity)result.Result;

            return new User
            {
                Email = userEntity.RowKey.Substring(UserRowKeyPrefix.Length),
                CustomData = userMetadataEntity.CustomData
            };
        }
    }
}