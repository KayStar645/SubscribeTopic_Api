using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ThesisRegistration;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.ThesisRegistrations.Event
{
    public class AfterDeleteThesisRegistrationDeletePointEvent : INotification
    {
        public ThesisRegistration _thesisRegistration { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterDeleteThesisRegistrationDeletePointEvent(ThesisRegistration thesisRegistration, IUnitOfWork unitOfWork)
        {
            _thesisRegistration = thesisRegistration;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterDeleteThesisRegistrationDeletePointHandler : INotificationHandler<AfterDeleteThesisRegistrationDeletePointEvent>
    {
        public async Task Handle(AfterDeleteThesisRegistrationDeletePointEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var thesisId = pEvent._thesisRegistration.ThesisId;
            var groupId = pEvent._thesisRegistration.GroupId;

            // Giảng viên hướng dẫn
            var teacherInstruct = await pEvent._unitOfWork.Repository<ThesisInstruction>()
                                    .Query()
                                    .Where(x => x.ThesisId == thesisId)
                                    .Select(x => x.TeacherId)
                                    .ToListAsync();
            // Giảng viên phản biện
            var teacherReview = await pEvent._unitOfWork.Repository<ThesisReview>()
                                        .Query()
                                        .Where(x => x.ThesisId == thesisId)
                                        .Select(x => x.TeacherId)
                                        .ToListAsync();

            // Thành viên nhóm
            var studentJoin = await pEvent._unitOfWork.Repository<StudentJoin>()
                                .Query()
                                .Where(x => x.GroupId == groupId)
                                .ToListAsync();
            if (teacherInstruct != null)
            {
                foreach (var teacherId in teacherInstruct)
                {
                    foreach (var student in studentJoin)
                    {
                        var point = await pEvent._unitOfWork.Repository<Point>()
                            .FirstOrDefaultAsync(x => x.StudentJoinId == student.Id &&
                                                      x.TeacherId == teacherId);
                        await pEvent._unitOfWork.Repository<Point>().DeleteAsync(point);
                        await pEvent._unitOfWork.Save();
                    }
                }
            }

            if (teacherReview != null)
            {
                foreach (var teacherId in teacherReview)
                {
                    foreach (var student in studentJoin)
                    {
                        var point = await pEvent._unitOfWork.Repository<Point>()
                            .FirstOrDefaultAsync(x => x.StudentJoinId == student.Id &&
                                                      x.TeacherId == teacherId);
                        await pEvent._unitOfWork.Repository<Point>().DeleteAsync(point);
                        await pEvent._unitOfWork.Save();
                    }
                }
            }
        }

    }
}
