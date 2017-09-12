using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class UserMetadataEntity : TableEntity
    {
        public string CustomData { get; set; }
    }
}
