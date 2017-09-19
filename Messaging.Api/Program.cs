using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Messaging.Api
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Configures and runs the web server.
        /// </summary>
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}
