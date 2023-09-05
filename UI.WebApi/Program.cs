using API.WebApi.Middleware;
using Autofac.Extensions.DependencyInjection;
using Cqrs.Hosts;
using Infrastructure.Identity;
using Microsoft.OpenApi.Models;

namespace UI.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                   .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                   .ConfigureWebHostDefaults(webHostBuilder => {
                       webHostBuilder
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                   })
                   .Build();

            host.Run();

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
