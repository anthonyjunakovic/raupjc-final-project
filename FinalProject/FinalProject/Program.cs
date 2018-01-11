using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace FinalProject
{
    public static class Program
    {
        public const string Address = "pinboard.azurewebsites.net/";
        public const string UrlAddress = "https://" + Address;

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
