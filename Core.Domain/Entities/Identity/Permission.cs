using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class Permission : BaseAuditableEntity
    {
        public string? Name { get; set; }

        [NotMapped]
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
