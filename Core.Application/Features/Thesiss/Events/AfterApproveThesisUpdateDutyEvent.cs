using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterApproveThesisUpdateDutyEvent : INotification
    {
        public Thesis _thesis { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterApproveThesisUpdateDutyEvent(Thesis thesis, IUnitOfWork unitOfWork)
        {
            _thesis = thesis;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterApproveThesisUpdateDutyHandler : INotificationHandler<AfterApproveThesisUpdateDutyEvent>
    {
        public async Task Handle(AfterApproveThesisUpdateDutyEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();
            var dutyId = pEvent._thesis.DutyId;

            var departmentDuty = await pEvent._unitOfWork.Repository<Duty>()
                .FirstOrDefaultAsync(x => x.Id == dutyId && x.Type == Duty.TYPE_DEPARTMENT);
            departmentDuty.NumberThesisComplete = departmentDuty.NumberThesisComplete == null ?
                                                    1 : departmentDuty.NumberThesisComplete + 1;
            await pEvent._unitOfWork.Repository<Duty>().UpdateAsync(departmentDuty);
            await pEvent._unitOfWork.Save();

            var facultyDuty = await pEvent._unitOfWork.Repository<Duty>()
                .FirstOrDefaultAsync(x => x.Id == departmentDuty.DutyId && x.Type == Duty.TYPE_FACULTY);
            facultyDuty.NumberThesisComplete = facultyDuty.NumberThesisComplete == null ?
                                                    1 : facultyDuty.NumberThesisComplete + 1;
            await pEvent._unitOfWork.Repository<Duty>().UpdateAsync(facultyDuty);
            await pEvent._unitOfWork.Save();
        }

    }
}
