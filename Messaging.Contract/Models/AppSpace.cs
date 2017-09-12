using System;
using System.Collections.Generic;

namespace Messaging.Contract.Models
{
    public class AppSpace
    {
        public Guid Id { get; set; }

        public ICollection<Channel> Channels { get; set; }
    }
}