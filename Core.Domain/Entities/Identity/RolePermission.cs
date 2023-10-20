using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class RolePermission : BaseAuditableEntity
    {
        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        public int? PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission? Permission { get; set; }
    }
}
