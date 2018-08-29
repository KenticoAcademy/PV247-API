using System;

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
        public string CustomData { get; set; }
    }
}
