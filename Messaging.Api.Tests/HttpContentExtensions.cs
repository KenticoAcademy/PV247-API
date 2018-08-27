using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Messaging.Api.Tests
{
    internal static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var contentString = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(contentString);
        }
    }
}
