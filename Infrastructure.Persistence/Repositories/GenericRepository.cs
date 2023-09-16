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

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext
                .Set<T>()
                .ToListAsync();
        }
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        // Query
        public IQueryable<T> Entities => _dbContext.Set<T>();

        public IQueryable<T> GetAllInclude(Expression<Func<T, object>> includeProperties = null)
        {
            var query = _dbContext.Set<T>().AsNoTracking();

            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }

            return query;
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public IQueryable<T> GetByIdInclude(int? id, params Expression<Func<T, object>>[] includeProperties)
        {
            var query = _dbContext.Set<T>().AsQueryable();

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

        public IQueryable<T> AddInclude(IQueryable<T> query, Expression<Func<T, object>> includeProperties = null)
        {
            if (includeProperties != null)
            {
                query = query.Include(includeProperties);
            }
            return query;
        }
    }
}
