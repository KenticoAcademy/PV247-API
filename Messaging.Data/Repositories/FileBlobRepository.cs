using System;
using System.IO;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Messaging.Data.Repositories
{
    internal class FileBlobRepository : IFileBlobRepository
    {
        private readonly CloudBlobContainer _container;

        public FileBlobRepository(StorageClientFactory clientFactory)
        {
            _container = clientFactory.GetBlobClient()
                .GetContainerReference("files");
        }

        public async Task UploadFile(FileMetadata fileMetadata, Stream fileStream)
        {
            var blockBlob = await GetBlockBlob(fileMetadata);

            await blockBlob.UploadFromStreamAsync(fileStream);
        }

        public async Task<Uri> GetDownloadUrl(FileMetadata fileMetadata)
        {
            var blockBlob = await GetBlockBlob(fileMetadata);
            var token = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddYears(1)
            });

            // TODO: Cache this
            return new Uri(blockBlob.Uri + token);
        }

        private async Task<CloudBlockBlob> GetBlockBlob(FileMetadata file)
        {
            if (!await _container.ExistsAsync())
            {
                await _container.CreateIfNotExistsAsync();
            }

            return _container.GetBlockBlobReference($"{file.Id.ToString().ToLower()}/{file.Name}");
        }
    }
}