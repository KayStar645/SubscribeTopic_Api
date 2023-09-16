using Core.Domain.Entities;
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

        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Faculty> Facultys { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
