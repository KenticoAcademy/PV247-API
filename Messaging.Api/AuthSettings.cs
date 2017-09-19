using Microsoft.IdentityModel.Tokens;

namespace Messaging.Api
{
    /// <summary>
    /// Settings for the JWT bearer authentication.
    /// </summary>
    public class AuthSettings
    {
        /// <summary>
        /// Key used fro signing of the tokens.
        /// </summary>
        public SecurityKey TokenSigningKey { get; set; }
    }
}