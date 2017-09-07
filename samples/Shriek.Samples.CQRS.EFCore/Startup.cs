using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shriek.EventStorage.EFCore;
using Shriek.Mvc;
using Shriek.Samples;
using Shriek.Samples.CQRS.EFCore;
using Shriek.Samples.Queries;
using Shriek.Samples.Repositories;
using Shriek.Samples.Services;

namespace Shriek.Samples.CQRS.EFCore
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().UseWebApiProxy(option =>
            {
                option.AddWebApiProxy<SampleApiProxy>();
            });

            services.AddShriek();

            //事件存储
            services.AddEFCoreEventStorage(options =>
                options.UseSqlite(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder { DataSource = "shriek.event.db" }.ToString()));

            //真实数据库
            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlite(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder { DataSource = "shriek.real.db" }.ToString()));

            services.AddScoped<ITodoQuery, TodoQuery>();
            services.AddScoped<ITodoRepository, TodoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}