namespace Core.Application.Models.Identity.Auths
{
    public class AssignRoleRequest
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }
    }
}
