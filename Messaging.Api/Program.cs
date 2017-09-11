using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Messaging.Api
{
    public class Program
    {
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
