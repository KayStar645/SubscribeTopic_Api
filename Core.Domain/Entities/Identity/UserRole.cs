using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class UserRole : BaseAuditableEntity
    {
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }


        public int? RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
