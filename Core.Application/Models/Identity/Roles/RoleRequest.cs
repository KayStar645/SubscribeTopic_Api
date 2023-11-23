namespace Core.Application.Models.Identity.Roles
{
    public class RoleRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Message { get; set; }

        public List<string>? PermissionsName { get; set; }
    }
}
