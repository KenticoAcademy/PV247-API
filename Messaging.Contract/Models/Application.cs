using System;
using System.Collections.Generic;

namespace Messaging.Contract.Models
{
    public class Application
    {
        public Guid Id { get; set; }

        public List<Channel> Channels { get; set; }
    }
}