using Core.Application.Contracts.Persistence;
using Core.Application.Features.RegistrationPeriods.Requests.Queries;
using Core.Domain.Entities;

namespace Core.Application.Interfaces.Repositories
{
    public interface IRegistrationPeriodRepository : IGenericRepository<RegistrationPeriod>
    {
        Task<RegistrationPeriod> GetCurrentRegistrationPeriodAsync(CurrentRegistrationPeriodRequest request);
    }
}
