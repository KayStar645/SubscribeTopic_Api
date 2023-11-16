using Core.Application.Responses;
using System.Reflection;

namespace Core.Application.Interfaces.Identity
{
    public interface IPermissionService
    {
        Task<Result<List<string>>> GetList(Assembly pAssembly);

        Task<Result<List<string>>> Create(List<string> pPermissions);
    }
}
