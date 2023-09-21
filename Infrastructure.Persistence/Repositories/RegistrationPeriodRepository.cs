using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RegistrationPeriodRepository : GenericRepository<RegistrationPeriod>, IRegistrationPeriodRepository
    {
        private readonly SubscribeTopicDbContext _dbContext;

        public RegistrationPeriodRepository(SubscribeTopicDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RegistrationPeriod> GetCurrentRegistrationPeriodAsync(CurrentRegistrationPeriodRequest request)
        {
            return await _dbContext.Set<RegistrationPeriod>()
            .Where(p =>
                    p.SchoolYear == request.schoolYear &&
                    p.Semester == request.semester &&
                    p.FacultyId == request.facultyId &&
                    p.IsDeleted == false)
            .FirstOrDefaultAsync();
        }
    }
}
