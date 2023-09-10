using Core.Application.Contracts.Identity;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<Users> _userManager;

        public UserService(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        // Hiện tại chưa dùng nè
        //public async Task<Employee> GetEmployee(string userId)
        //{
        //    var employee = await _userManager.FindByIdAsync(userId);
        //    return new Employee
        //    {
        //        Id = employee.Id,
        //    };
        //}

        //// Hiện tại chưa dùng nè
        //public async Task<List<Employee>> GetEmployees()
        //{
        //    var employees = await _userManager.GetUsersInRoleAsync("Employee");
        //    return employees.Select(q => new Employee { 
        //        Id = q.Id,
        //    }).ToList();
        //}
    }
}
