using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Messaging.Api
{
    internal static class HttpContextExtensions
    {
        public static string GetCurrentUserId(this HttpContext httpContext)
        {
            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}