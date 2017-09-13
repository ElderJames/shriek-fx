using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shriek.ServiceProxy.Http.Server
{
    public class MvcStartup
    {
        public MvcStartup()
        {
            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddFormatterMappings();
            //.AddWebApiProxy();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode != StatusCodes.Status200OK)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "text/plain";
                    await context.Response.WriteAsync("server is ok");
                }
            });
        }
    }
}