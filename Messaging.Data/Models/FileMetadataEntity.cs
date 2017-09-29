using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class FileMetadataEntity : TableEntity
    {
        public string CreatedBy { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public long FileSize { get; set; }
    }
}