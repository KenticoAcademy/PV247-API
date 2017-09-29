using Messaging.Contract.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data
{
    internal class StorageClientFactory
    {
        private readonly CloudTableClient _tableClient;
        private readonly CloudBlobClient _blobClient;

        public StorageClientFactory(IOptions<StorageSettings> settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.Value.StorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        public CloudTableClient GetTableClient() => _tableClient;

        public CloudBlobClient GetBlobClient() => _blobClient;
    }
}