namespace Core.Domain.Entities.Identity
{
    public class RolePermissions
    {
        public string Id { get; set; }

        public string RoleId { get; set; }
        public Roles Roles { get; set; }

        public string PermissionId { get; set; }
        public Permissions Permissions { get; set; }

        public RolePermissions() 
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
