
using Core.Domain.Common;

namespace Core.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity;

        Task<int> Save(CancellationToken cancellationToken);

        Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}
