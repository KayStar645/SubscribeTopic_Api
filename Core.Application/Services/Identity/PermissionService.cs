using Core.Application.Interfaces.Identity;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services.Identity
{
    public class PermissionService : IPermissionService
    {
        // Xem lại cái này: UserManager, RoleManager
        private readonly UserManager<Permission> _permissionManager;

        public PermissionService(UserManager<Permission> permissionManager)
        {
            _permissionManager = permissionManager;
        }
    }
}
