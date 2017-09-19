using Messaging.Contract.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data
{
    internal class TableClientFactory
    {
        private readonly CloudTableClient _tableClient;

        public TableClientFactory(IOptions<StorageSettings> settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.Value.StorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
        }

        public CloudTableClient GetTableClient() => _tableClient;
    }
}