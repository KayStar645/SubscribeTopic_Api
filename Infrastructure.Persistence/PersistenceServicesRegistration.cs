using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class PersistenceServicesRegistration
    {
        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SubscribeTopicDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("SubscribeTopicConnectionString"))
            );


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IRegistrationPeriodRepository, RegistrationPeriodRepository>();

            return services;
        }
    }
}
