using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Domain.Common;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SubscribeTopicDbContext _dbContext;
        private Hashtable _repositories;
        private bool disposed;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UnitOfWork(SubscribeTopicDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._httpContextAccessor = httpContextAccessor;
        }

        public IGenericRepository<T> Repository<T>() where T : BaseAuditableEntity
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);

                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public Task Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
            return Task.CompletedTask;
        }

        public async Task<int> Save(CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var id = httpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                return await _dbContext.SaveChangesAsync(id);
            }
            else
            {
                return await _dbContext.SaveChangesAsync("");
            }
        }

        public async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync("SKIP");
        }


        public Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    _dbContext.Dispose();
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }
    }
}
