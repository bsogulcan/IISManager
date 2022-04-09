using System;
using System.Net;
using System.Net.Sockets;
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

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}