using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Council;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Councils.Events
{
    public class AfterSelectThesisForCouncilCreateOrUpdatePointEvent : INotification
    {
        public Council _council { get; set; }

        public List<ThesisScheduleDto> _listThesis { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterSelectThesisForCouncilCreateOrUpdatePointEvent(Council council, List<ThesisScheduleDto> listThesis, 
            IUnitOfWork unitOfWork)
        {
            _council = council;
            _listThesis = listThesis;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterSelectThesisForCouncilCreateOrUpdatePointHandler : INotificationHandler<AfterSelectThesisForCouncilCreateOrUpdatePointEvent>
    {
        public async Task Handle(AfterSelectThesisForCouncilCreateOrUpdatePointEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();
            foreach (var thesis in pEvent._listThesis)
            {
                var councilId = pEvent._council.Id;
                var thesisId = thesis.ThesisId;
                var groupId = await pEvent._unitOfWork.Repository<ThesisRegistration>()
                                        .Query()
                                        .Where(x => x.ThesisId == thesisId)
                                        .Select(x => x.GroupId)
                                        .FirstOrDefaultAsync();
                if (groupId == null)
                {
                    continue;
                }
                // Thành viên nhóm
                var studentJoin = await pEvent._unitOfWork.Repository<StudentJoin>()
                                    .Query()
                                    .Where(x => x.GroupId == groupId)
                                    .ToListAsync();

                // Giảng viên hội đồng
                var teacher = await pEvent._unitOfWork.Repository<Commissioner>()
                                    .Query()
                                    .Where(x => x.CouncilId == councilId)
                                    .Select(x => x.TeacherId)
                                    .ToListAsync();
                if (teacher != null && studentJoin != null)
                {
                    foreach (var teacherId in teacher)
                    {
                        foreach (var student in studentJoin)
                        {
                            var exists = await pEvent._unitOfWork.Repository<Point>()
                                            .AnyAsync(x => x.TeacherId == teacherId &&
                                                           x.StudentJoinId == student.Id &&
                                                           x.Type == Point.TYPE_COUNCIL);
                            if (exists == false)
                            {
                                var point = new Point
                                {
                                    Type = Point.TYPE_COUNCIL,
                                    Scores = 0,
                                    TeacherId = teacherId,
                                    StudentJoinId = student.Id
                                };
                                await pEvent._unitOfWork.Repository<Point>().AddAsync(point);
                                await pEvent._unitOfWork.Save();
                            }
                        }
                    }
                }    
            }

        }

    }
}
