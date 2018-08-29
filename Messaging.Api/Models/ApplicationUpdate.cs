using Newtonsoft.Json.Linq;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Model for the application update
    /// </summary>
    public class ApplicationUpdate
    {
        /// <summary>
        /// New application custom metadata
        /// </summary>
        public JObject CustomData { get; set; }
    }
}
