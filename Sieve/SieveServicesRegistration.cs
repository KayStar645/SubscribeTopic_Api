using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services.Interface;
using Sieve.Services;

namespace Sieve
{
    public static class SieveServicesRegistration
    {
        public static IServiceCollection ConfigureSieveServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISieveProcessor, SieveProcessor>();

            return services;
        }
    }
}
