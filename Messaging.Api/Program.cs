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
        /// Configures the web host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseApplicationInsights();

        /// <summary>
        /// Runs the web server.
        /// </summary>
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            host.Run();
        }
    }
}
