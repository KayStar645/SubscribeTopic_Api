using Core.Application.Interfaces.Identity;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Roles> _roleManager;

        public RoleService(RoleManager<Roles> roleManager)
        {
            _roleManager = roleManager;
        }
    }
}
