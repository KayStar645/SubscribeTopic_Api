using Core.Application.Interfaces.Identity;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services.Identity
{
    public class PermissionService : IPermissionService
    {
        // Xem lại cái này: UserManager, RoleManager
        private readonly UserManager<Permissions> _permissionManager;

        public PermissionService(UserManager<Permissions> permissionManager)
        {
            _permissionManager = permissionManager;
        }
    }
}
