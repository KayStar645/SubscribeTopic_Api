using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities.Identity
{
    public class Roles : IdentityRole<string>
    {
        public Roles()
        {
            RolePermissions = new HashSet<RolePermissions>();
            UserRoles = new HashSet<UserRoles>();
        }

        public ICollection<RolePermissions> RolePermissions { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
