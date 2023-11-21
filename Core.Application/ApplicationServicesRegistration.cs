using Core.Application.Contracts.Identity;
using Core.Application.Features.Base.Handlers.Commands;
using Core.Application.Interfaces.Identity;
using Core.Application.Interfaces.Services;
using Core.Application.Models.Identity.Auths;
using Core.Application.Services.GoogleDrive;
using Core.Application.Services.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddTransient(typeof(IRequestHandler<,>), typeof(DeleteBaseCommandHandler<>));

            services.AddScoped<JwtSettings>();
            services.AddScoped<IPasswordHasher<Core.Domain.Entities.Identity.User>, PasswordHasher<Core.Domain.Entities.Identity.User>>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();


            return services;
        }
    }
}
