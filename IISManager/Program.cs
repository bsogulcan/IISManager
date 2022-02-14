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
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            
            return WebHost.CreateDefaultBuilder(args).UseUrls(args[0])
                .UseStartup<Startup>();
        }
    }
}