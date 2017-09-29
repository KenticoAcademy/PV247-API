using System;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Data.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Repositories
{
    internal class FileMetadataRepository : IFileMetadataRepository
    {
        private const string GlobalPartitionKey = "Global";
        private const string RowKeyPrefix = "F;";
        private readonly CloudTable _table;

        public FileMetadataRepository(StorageClientFactory clientFactory)
        {
            _table = clientFactory.GetTableClient()
                .GetTableReference("DataTable");
        }

        public async Task<FileMetadata> CreateFileMetadata(FileMetadata fileMetadata)
        {
            var entity = new FileMetadataEntity
            {
                PartitionKey = GlobalPartitionKey,
                RowKey = RowKeyPrefix + fileMetadata.Id,
                CreatedBy = fileMetadata.CreatedBy,
                Name = fileMetadata.Name,
                Extension = fileMetadata.Extension,
                FileSize = fileMetadata.FileSize
            };

            var result = await _table.ExecuteAsync(TableOperation.Insert(entity));
            var created = (FileMetadataEntity) result.Result;

            return ToDto(created);
        }

        public async Task<FileMetadata> GetFileMetadata(Guid fileId)
        {
            var result = await _table.ExecuteAsync(TableOperation.Retrieve<FileMetadataEntity>(GlobalPartitionKey, RowKeyPrefix + fileId));
            var entity = (FileMetadataEntity) result.Result;

            return ToDto(entity);
        }

        private static FileMetadata ToDto(FileMetadataEntity entity) => new FileMetadata
        {
            Id = Guid.Parse(entity.RowKey.Split(';').Last()),
            CreatedBy = entity.CreatedBy,
            Name = entity.Name,
            Extension = entity.Extension,
            FileSize = entity.FileSize
        };
    }
}