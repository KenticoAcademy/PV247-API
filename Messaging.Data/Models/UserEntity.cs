using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Models
{
    internal class UserEntity : TableEntity
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
    }
}