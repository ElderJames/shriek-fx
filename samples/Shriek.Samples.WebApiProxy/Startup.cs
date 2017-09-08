using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shriek.Mvc;
using Shriek.Samples.WebApiProxy.Contacts;

namespace Shriek.Samples.WebApiProxy
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddFormatterMappings()
                .UseWebApiProxy(option =>
            {
                option.AddWebApiProxy<SampleApiProxy>();
            });
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
                    await context.Response.WriteAsync("server is ok");
                }
            });
        }
    }
}