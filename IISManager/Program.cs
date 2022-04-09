using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IISManager
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var ipAddress = config.GetSection("App:IpAddress").Value;
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new Exception("Ip Address configuration not found at appsettings.json");
            }

            var port = config.GetSection("App:Port").Value;
            if (string.IsNullOrEmpty(port))
            {
                throw new Exception("Port configuration not found at appsettings.json");
            }

            return WebHost.CreateDefaultBuilder(args).UseUrls(ipAddress + ":" + port)
                .UseStartup<Startup>();
        }
    }
}