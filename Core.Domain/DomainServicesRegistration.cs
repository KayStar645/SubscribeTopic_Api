using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Domain
{
    public static class DomainServicesRegistration
    {
        public static IServiceCollection ConfigureDomainServices(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
