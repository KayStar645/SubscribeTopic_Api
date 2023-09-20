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
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> GetByIdAsync(int? id)
        {
            return await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false && x.Id == id)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            T exist = await _dbContext.Set<T>()
                .Where(x => x.IsDeleted == false && x.Id == entity.Id)
                .FirstOrDefaultAsync();
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return entity;
        }

        public virtual Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        // Query
        public virtual IQueryable<T> Entities => _dbContext.Set<T>();

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
