using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly SubscribeTopicDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserRepository(SubscribeTopicDbContext dbContext, IPasswordHasher<User> passwordHasher) : base(dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<User>> Get()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<bool> CreateAsync(User user)
        {
            var hashedPassword = _passwordHasher.HashPassword(user, user.Password);
            user.Password = hashedPassword;

            await _dbContext.Set<User>().AddAsync(user);

            try
            {
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User> FindByNameAsync(string userName) => await _dbContext.Set<User>().Where(x => x.UserName == userName).FirstOrDefaultAsync();

        public async Task<(Faculties faculty, int type)> GetFacultyAsync(User user)
        {
            var studentFaculty = await _dbContext.Set<Student>()
                                    .Where(x => x.User.UserName == user.UserName)
                                    .Select(x => x.Major.Industry.Faculty)
                                    .FirstOrDefaultAsync();

            var teacherFaculty = await _dbContext.Set<Teacher>()
                                    .Where(x => x.User.UserName == user.UserName)
                                    .Select(x => x.Department.Faculty)
                                    .FirstOrDefaultAsync();

            int type = -1;

            if (studentFaculty != null)
            {
                type = 0;
            }
            else if (teacherFaculty != null)
            {
                type = 1;
            }

            return (studentFaculty ?? teacherFaculty, type);
        }

        public async Task<List<Permission>> GetPermissionsAsync(User user)
        {
            var permissions = await _dbContext.Set<UserPermission>()
                                        .Where(x => x.UserId == user.Id)
                                        .Select(x => x.Permission)
                                        .Distinct()
                                        .ToListAsync();

            var userRoles = await _dbContext.Set<UserRole>()
                                        .Where(ur => ur.Role != null && ur.UserId == user.Id)
                                        .ToListAsync();

            foreach (var userRole in userRoles)
            {
                var permissionsWithRole = await _dbContext.RolePermissions
                                            .Include(x => x.Permission)
                                            .Where(x => x.RoleId == userRole.RoleId)
                                            .Select(x => x.Permission)
                                            .ToListAsync();
                permissions = permissions.Union(permissionsWithRole).ToList();
            }

            return permissions;
        }

        public async Task<List<Role>> GetRolesAsync(User user)
        {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return await _dbContext.Set<UserRole>()
                                    .Include(x => x.Role)
                                    .Where(x => x.UserId == user.Id)
                                    .Select(x => x.Role)
                                    .ToListAsync();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        public async Task<bool> PasswordSignInAsync(string userName, string password)
        {
            var user = await FindByNameAsync(userName);

            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

                return result == PasswordVerificationResult.Success;
            }

            return false;
        }

        public async Task<object> GetCustomerByUserName(string userName, int type)
        {
            if(type == 0)
            {
                return await _dbContext.Students.Where(x => x.InternalCode == userName).FirstOrDefaultAsync();
            }   
            else if(type == 1)
            {
                return await _dbContext.Teachers.Where(x => x.InternalCode == userName).FirstOrDefaultAsync();
            }
            return null;
        }
    }
}
