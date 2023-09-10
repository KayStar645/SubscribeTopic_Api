using Microsoft.AspNetCore.Identity;

namespace Core.Domain.Entities.Identity
{
    public class UserRoles
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public Users Users { get; set; }

        public string RoleId { get; set; }
        public Roles Roles { get; set; }

        public UserRoles() 
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
