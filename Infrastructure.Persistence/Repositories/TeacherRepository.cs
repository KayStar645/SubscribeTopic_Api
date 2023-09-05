using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        private readonly SubscribeTopicDbContext _dbContext;

        public TeacherRepository(SubscribeTopicDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
