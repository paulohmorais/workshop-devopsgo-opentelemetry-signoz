using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace apicsharp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "REDIS-HOST:6379"; // IP da sua máquina - Redis server address
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World");
                });

                endpoints.MapGet("/page1", async context =>
                {
                    await context.Response.WriteAsync("Hello Page1");
                });

                endpoints.MapGet("/page2", async context =>
                {
                    await context.Response.WriteAsync("Hello Page2");
                });

                endpoints.MapGet("/healthcheck", async context =>
                {
                    await context.Response.WriteAsync("ok");
                });

                endpoints.MapGet("/errortest", async context =>
                {
                    context.Response.StatusCode = 500;
                    await Task.CompletedTask;
                });

                endpoints.MapGet("/api1", async context =>
                {
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users/");
                    await context.Response.WriteAsync($"[{response}]");
                });

                endpoints.MapGet("/api2", async context =>
                {
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetStringAsync("https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8");
                    await context.Response.WriteAsync($"[{response}]");
                });

                endpoints.MapGet("/api1-cached", async context =>
                {
                    var cache = context.RequestServices.GetRequiredService<IDistributedCache>();
                    var cachedData = await cache.GetStringAsync("api1-cached");

                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        await context.Response.WriteAsync($"[{cachedData}]");
                    }
                    else
                    {
                        var httpClient = new HttpClient();
                        var response = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users/");
                        await cache.SetStringAsync("api1-cached", response, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300)
                        });
                        await context.Response.WriteAsync($"[{response}]");
                    }
                });

                endpoints.MapGet("/api2-cached", async context =>
                {
                    var cache = context.RequestServices.GetRequiredService<IDistributedCache>();
                    var cachedData = await cache.GetStringAsync("api2-cached");

                    if (!string.IsNullOrEmpty(cachedData))
                    {
                        await context.Response.WriteAsync($"[{cachedData}]");
                    }
                    else
                    {
                        var httpClient = new HttpClient();
                        var response = await httpClient.GetStringAsync("https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8");
                        await cache.SetStringAsync("api2-cached", response, new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                        });
                        await context.Response.WriteAsync($"[{response}]");
                    }
                });
            });
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:3000");
                });
    }
}
