using System;

namespace Messaging.Api.Models
{
    /// <summary>
    /// Response model for the login.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT for the Bearer authentication
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration in ISO format.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
