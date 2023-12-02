using Core.Domain.Entities;
using Core.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class SubscribeTopicDbContext : AuditableDbContext
    {
        public SubscribeTopicDbContext(DbContextOptions<SubscribeTopicDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubscribeTopicDbContext).Assembly);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculties> Faculties { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Industry> Industries { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RegistrationPeriod> RegistrationPeriods { get; set; }
        public DbSet<StudentJoin> StudentJoins { get; set; }
        public DbSet<DepartmentDuty> DepartmentDuties { get; set; }
        public DbSet<FacultyDuty> FacultyDuties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Thesis> Thesiss { get; set; }
        public DbSet<ThesisInstruction> ThesisInstructions { get; set; }
        public DbSet<ThesisMajor> ThesisMajors { get; set; }
        public DbSet<ThesisReview> ThesisReviews { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ThesisRegistration> ThesisRegistrations { get; set; }
        public DbSet<Job> Jobs { get; set; }
    }
}
