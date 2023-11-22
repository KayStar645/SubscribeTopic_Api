using Core.Application.Contracts.Persistence;
using Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseAuditableEntity
    {
        private readonly SubscribeTopicDbContext _dbContext;

        public GenericRepository(SubscribeTopicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Async

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .ToListAsync();
        }
        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(predicate);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public virtual async Task<T> GetByIdAsync(int? id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false && x.Id == id)
                .FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            T exist = await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false && x.Id == entity.Id)
                .FirstOrDefaultAsync();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            return entity;
        }

        public virtual async Task<bool> UpdateRangeAsync(List<T> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    _dbContext.Entry(entity).State = EntityState.Modified;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public virtual Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .AnyAsync(predicate);
        }

        // Query
        public virtual IQueryable<T> Entities => _dbContext.Set<T>();

        public virtual IQueryable<T> Query()
        {
            var query = _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .AsNoTracking();

            return query;
        }

        public virtual IQueryable<T> GetAllInclude(Expression<Func<T, object>> includeProperties = null)
        {
            var query = _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .AsNoTracking();

            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }

            return query;
        }

        public virtual IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .Where(predicate);
        }

        public virtual IQueryable<T> GetByIdInclude(int? id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .AsQueryable();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (id != null)
            {
                query = query.Where(e => e.Id == id);
            }

            return query;
        }

        public virtual IQueryable<T> AddInclude(IQueryable<T> query, Expression<Func<T, object>> includeProperties = null)
        {
            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }
            return query;
        }
    }
}
