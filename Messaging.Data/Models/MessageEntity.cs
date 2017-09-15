using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class MessageEntity : TableEntity
    {
        public string Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public string CustomData { get; set; }
    }
}