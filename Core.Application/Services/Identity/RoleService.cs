using Core.Application.Interfaces.Identity;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }
    }
}
