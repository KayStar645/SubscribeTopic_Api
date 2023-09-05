using Core.Application.Contracts.Identity;
using Core.Application.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
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
