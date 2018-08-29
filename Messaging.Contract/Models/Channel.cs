using System;
using Newtonsoft.Json.Linq;

namespace Messaging.Contract.Models
{
    public class Channel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public JObject CustomData { get; set; }
    }
}