using System;
using System.Collections.Generic;

namespace Messaging.Contract.Models
{
    public class Application
    {
        public Guid Id { get; set; }

        public ICollection<Channel> Channels { get; set; }
    }
}