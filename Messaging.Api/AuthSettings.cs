using Microsoft.IdentityModel.Tokens;

namespace Messaging.Api
{
    public class AuthSettings
    {
        public SecurityKey TokenSigningKey { get; set; }
    }
}