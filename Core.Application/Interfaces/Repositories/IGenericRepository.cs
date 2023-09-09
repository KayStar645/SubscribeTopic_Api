using Core.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace Core.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        // Async
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int? id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        // Query
        IQueryable<T> Entities { get; }
        IQueryable<T> GetAllInclude(Expression<Func<T, object>> includeProperties = null);
        IQueryable<T> GetByIdInclude(int? id, params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> AddInclude(IQueryable<T> query, Expression<Func<T, object>> includeProperties = null);
        IQueryable<T> GetAllSieve();
        
        
        
    }
}
