using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FinalProject
{
    public static class Program
    {
        public const string Address = "192.168.1.104:51208/";
        public const string UrlAddress = "http://" + Address;

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
        }
    }
}
