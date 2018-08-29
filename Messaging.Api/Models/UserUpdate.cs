using Newtonsoft.Json.Linq;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Model for the user update.
    /// </summary>
    public class UserUpdate
    {
        /// <summary>
        /// Custom user metadata
        /// </summary>
        public JObject CustomData { get; set; }
    }
}
