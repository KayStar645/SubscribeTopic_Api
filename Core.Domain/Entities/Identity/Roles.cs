using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities.Identity
{
    public class Roles : IdentityRole<string>
    {
        public Roles()
        {
            Id = Guid.NewGuid().ToString();
            RolePermissions = new HashSet<RolePermissions>();
            UserRoles = new HashSet<UserRoles>();
        }

        public ICollection<RolePermissions> RolePermissions { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
