using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class UserPermission : BaseAuditableEntity
    {
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission? Permission { get; set; }
    }
}
