using IISManager.Managers.FileManager;
using IISManager.Managers.SiteManagers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IISManager
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<ISiteManager, SiteManager>();
            services.AddTransient<IFileManager, FileManager>();

            /*services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(o =>
                {
                    o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                });*/
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}