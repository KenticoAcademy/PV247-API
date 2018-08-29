using Newtonsoft.Json.Linq;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Channel update model
    /// </summary>
    public class ChannelUpdate
    {
        /// <summary>
        /// New channel name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// New channel custom data
        /// </summary>
        public JObject CustomData { get; set; }
    }
}
