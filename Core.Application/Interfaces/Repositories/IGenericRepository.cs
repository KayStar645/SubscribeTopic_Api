using Core.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace Core.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        // Async
        Task<List<T>> GetAllAsync();
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int? id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(List<T> entities);
        Task DeleteAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // Query
        IQueryable<T> Entities { get; }
        IQueryable<T> Query();
        IQueryable<T> GetAllInclude(Expression<Func<T, object>> includeProperties = null);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetByIdInclude(int? id, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AddInclude(IQueryable<T> query, Expression<Func<T, object>> includeProperties = null);
    }
}
