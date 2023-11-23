using Core.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public abstract class AuditableDbContext : DbContext
    {
        public AuditableDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual async Task<int> SaveChangesAsync(string username = "SYSTEM")
        {
            if(username != "SKIP")
            {
                foreach (var entry in base.ChangeTracker.Entries<BaseAuditableEntity>()
                .Where(q => q.State == EntityState.Added ||
                            q.State == EntityState.Modified ||
                            q.State == EntityState.Deleted))
                {
                    entry.Entity.LastModifiedDate = DateTime.Now;
                    entry.Entity.LastModifiedBy = username;

                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.DateCreated = DateTime.Now;
                        entry.Entity.CreatedBy = username;
                        entry.Entity.IsDeleted = false;
                    }

                    if (entry.State == EntityState.Deleted)
                    {
                        entry.Entity.IsDeleted = true;
                        entry.State = EntityState.Unchanged;
                    }
                }
            }    

            var result = await base.SaveChangesAsync();

            return result;
        }
    }
}
