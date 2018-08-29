using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Messaging.Contract.Models
{
    public class Application
    {
        public Guid Id { get; set; }

        public JObject CustomData { get; set; }

        public List<Channel> Channels { get; set; }
    }
}