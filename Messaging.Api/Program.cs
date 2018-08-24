using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Messaging.Api
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseApplicationInsights();

        /// <summary>
        /// Configures and runs the web server.
        /// </summary>
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            host.Run();
        }
    }
}
