using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities.Identity
{
    public class Users : IdentityUser<string>
    {
        public Users() 
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
