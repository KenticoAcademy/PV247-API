using System;
using Newtonsoft.Json.Linq;

namespace Messaging.Contract.Models
{
    public class Message
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public string UpdatedBy { get; set; }

        public JObject CustomData { get; set; }
    }
}
