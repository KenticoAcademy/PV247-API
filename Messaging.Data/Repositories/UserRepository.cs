using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
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
            throw new System.NotImplementedException();
        }

        public async Task<User> Upsert(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}