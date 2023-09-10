namespace Core.Domain.Entities.Identity
{
    public class UserPermissions
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public Users Users { get; set; }

        public string PermissionId { get; set; }
        public Permissions Permissions { get; set; }

        public UserPermissions() 
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
