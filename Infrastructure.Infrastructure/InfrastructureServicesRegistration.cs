using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            //services.AddTransient<IEmailSender, EmailSender>();

            return services;
        }
    }
}
