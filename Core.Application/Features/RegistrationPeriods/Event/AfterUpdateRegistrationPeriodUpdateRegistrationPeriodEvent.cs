using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.RegistrationPeriods.Event
{
    public class AfterUpdateRegistrationPeriodUpdateRegistrationPeriodEvent : INotification
    {
        public RegistrationPeriodDto _registrationPeriodDto { get; set; }
        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateRegistrationPeriodUpdateRegistrationPeriodEvent(RegistrationPeriodDto registrationPeriodDto, IUnitOfWork unitOfWork)
        {
            _registrationPeriodDto = registrationPeriodDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateRegistrationPeriodUpdateRegistrationPeriodHandler : INotificationHandler<AfterUpdateRegistrationPeriodUpdateRegistrationPeriodEvent>
    {
        public async Task Handle(AfterUpdateRegistrationPeriodUpdateRegistrationPeriodEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if(pEvent._registrationPeriodDto.IsActive == true)
            {
                // Update lại toàn bộ trạng thái khác thành false của khoa này
                var periods = await pEvent._unitOfWork.Repository<RegistrationPeriod>()
                                    .Query()
                                    .Where(x => x.Id != pEvent._registrationPeriodDto.Id && 
                                                x.FacultyId == pEvent._registrationPeriodDto.FacultyId)
                                    .ToListAsync();
                periods.ForEach(x => x.IsActive = false);

                await pEvent._unitOfWork.Repository<RegistrationPeriod>().UpdateRangeAsync(periods);
                await pEvent._unitOfWork.Save();

            }    
            
        }

    }
}
