using Core.Application.Models.Identity.Auths;
using Core.Application.Models.Identity.Roles;
using Core.Application.Responses;

namespace Core.Application.Interfaces.Identity
{
    public interface IRoleService
    {
        Task<Result<List<RoleResult>>> GetList();

        Task<Result<RoleResult>> GetDetail(int pId);

        Task<Result<RoleResult>> CreateAsync(RoleRequest pRequest);

        Task<Result<RoleResult>> UpdateAsync(RoleRequest pRequest);

        Task DeleteAsync(int pId);

        Task<Result<List<string>>> AssignRoles(AssignRoleRequest pRequest);

    }
}
