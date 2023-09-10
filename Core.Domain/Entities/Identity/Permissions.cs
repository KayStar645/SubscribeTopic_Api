namespace Core.Domain.Entities.Identity
{
    public class Permissions
    {
        public Permissions()
        {
            Id = Guid.NewGuid().ToString();
            RolePermissions = new HashSet<RolePermissions>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
