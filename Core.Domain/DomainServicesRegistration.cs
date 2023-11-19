using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services.Interface;
using Sieve.Services;

namespace Core.Domain
{
    public static class DomainServicesRegistration
    {
        public static IServiceCollection ConfigureDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISieveProcessor, ThesisSieveProcessor>();

            return services;
        }
    }
}
