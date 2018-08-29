using Newtonsoft.Json.Linq;

namespace Messaging.Contract.Models
{
    public class User
    {
        public string Email { get; set; }

        public JObject CustomData { get; set; }
    }
}