using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;

namespace Core.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> CreateAsync(User user);
        Task<User> FindByNameAsync(string userName);
        Task<(Faculties faculty, int type)> GetFacultyAsync(User user);
        Task<List<Permission>> GetPermissionsAsync(User user);
        Task<List<Role>> GetRolesAsync(User user);
        Task<bool> PasswordSignInAsync(string userName, string password);
    }
}
