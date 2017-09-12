using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Data.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly CloudTable _table;

        public UserRepository(IOptions<Settings> settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.Value.StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            _table = tableClient.GetTableReference("DataTable");
        }

        public async Task<User> Get(string email)
        {
            var result = await _table.ExecuteAsync(TableOperation.Retrieve<UserEntity>(email, "User"));
            var user = (UserEntity)result.Result;

            return new User
            {
                Id = user.Id,
                Email = user.Email
            };
        }

        public async Task<User> Upsert(User user)
        {
            var entity = new UserEntity
            {
                Id = user.Id,
                Email = user.Email
            };

            var result = await _table.ExecuteAsync(TableOperation.InsertOrReplace(entity));

            var insertedUser = (UserEntity)result.Result;

            return new User
            {
                Id = insertedUser.Id,
                Email = insertedUser.Email
            };
        }
    }
}