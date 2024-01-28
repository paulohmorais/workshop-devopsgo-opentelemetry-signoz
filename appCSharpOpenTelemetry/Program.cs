using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace apicsharp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("REDIS-HOST:6379")); // Configure Redis ConnectionMultiplexer

            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                    resource.AddService(serviceName: "sample-net-app"))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://SIGNOZ-HOST:4317");

            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
        }));
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
                                   var redis = app.ApplicationServices.GetRequiredService<IConnectionMultiplexer>();
                                   var cache = redis.GetDatabase();

                                   var cachedData = await cache.StringGetAsync("api1-cached");

                                   if (!cachedData.IsNullOrEmpty)
                                   {
                                       await context.Response.WriteAsync($"[{cachedData}]");
                                   }
                                   else
                                   {
                                       var httpClient = new HttpClient();
                                       var response = await httpClient.GetStringAsync("https://jsonplaceholder.typicode.com/users/");

                                       // Cache no Redis
                                       await cache.StringSetAsync("api1-cached", response, TimeSpan.FromSeconds(300));

                                       await context.Response.WriteAsync($"[{response}]");
                                   }
                               });

                endpoints.MapGet("/api2-cached", async context =>
                {
                    var redis = app.ApplicationServices.GetRequiredService<IConnectionMultiplexer>();
                    var cache = redis.GetDatabase();

                    var cachedData = await cache.StringGetAsync("api2-cached");

                    if (!cachedData.IsNullOrEmpty)
                    {
                        await context.Response.WriteAsync($"[{cachedData}]");
                    }
                    else
                    {
                        var httpClient = new HttpClient();
                        var response = await httpClient.GetStringAsync("https://mocki.io/v1/d4867d8b-b5d5-4a48-a4ab-79131b5809b8");

                        // Cache no Redis
                        await cache.StringSetAsync("api2-cached", response, TimeSpan.FromSeconds(5));

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
