using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class AppSpaceEntity: TableEntity
    {
        public string ChannelsJson { get; set; }
    }
}