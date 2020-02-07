using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Arista_LPS_WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // .UseUrls("http://CS_JAYP:4000")
                // .UseUrls("http://216.45.140.171:4000")
                .UseUrls("http://192.168.70.122:4000")
                // .UseUrls("http://192.168.70.141:4000")
                //.UseUrls("http://192.168.70.57:4000")
                .Build();
    }
}