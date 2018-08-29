using System;
using Newtonsoft.Json.Linq;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Application response model
    /// </summary>
    public class ApplicationResponse
    {
        /// <summary>
        /// Application ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Application custom metadata
        /// </summary>
        public JObject CustomData { get; set; }
    }
}
