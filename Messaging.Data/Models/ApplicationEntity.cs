using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class ApplicationEntity: TableEntity
    {
        public string ChannelsJson { get; set; }
    }
}