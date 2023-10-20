using Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Domain.Entities.Identity
{
    public class User : BaseAuditableEntity
    {
        public User() { }

        public User(string userName, string passWord)
        {
            UserName = userName;
            Password = passWord;
        }

        public string? UserName {  get; set; }

        public string? Password { get; set; }

        public Teacher? Teacher { get; set; }

        public Student? Student { get; set; }


        [NotMapped]
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        [NotMapped]
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
