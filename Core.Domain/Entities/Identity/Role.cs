using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class Role : BaseAuditableEntity
    {
        public string? Name { get; set; }

        public string? Message { get; set; }

        [NotMapped]
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [NotMapped]
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
