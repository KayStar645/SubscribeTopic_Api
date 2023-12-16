using Core.Application.Contracts.Persistence;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdatePointEvent : INotification
    {
        public Thesis _thesis { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateThesisCreateOrUpdatePointEvent(Thesis thesis, IUnitOfWork unitOfWork)
        {
            _thesis = thesis;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateThesisCreateOrUpdatePointHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdatePointEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdatePointEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var thesisId = pEvent._thesis.Id;
            var groupId = await pEvent._unitOfWork.Repository<ThesisRegistration>()
                                    .Query()
                                    .Where(x => x.ThesisId == thesisId)
                                    .Select(x => x.GroupId)
                                    .FirstOrDefaultAsync();
            if(groupId == null)
            {
                return;
            }    

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
            if (teacherInstruct != null && studentJoin != null)
            {
                foreach (var teacherId in teacherInstruct)
                {
                    foreach (var student in studentJoin)
                    {
                        var exists = await pEvent._unitOfWork.Repository<Point>()
                                        .AnyAsync(x => x.TeacherId == teacherId &&
                                                       x.StudentJoinId == student.Id &&
                                                       x.Type == Point.TYPE_INSTRUCTION);
                        if(exists == false)
                        {

                            var point = new Point
                            {
                                Type = Point.TYPE_INSTRUCTION,
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

            if (teacherReview != null&& studentJoin != null)
            {
                foreach (var teacherId in teacherReview)
                {
                    foreach (var student in studentJoin)
                    {
                        var exists = await pEvent._unitOfWork.Repository<Point>()
                                        .AnyAsync(x => x.TeacherId == teacherId &&
                                                       x.StudentJoinId == student.Id &&
                                                       x.Type == Point.TYPE_REVIEW);
                        if(exists == false)
                        {
                            var point = new Point
                            {
                                Type = Point.TYPE_REVIEW,
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

            // Xóa điểm của những giảng viên cũ đi
            var pointIOld = await pEvent._unitOfWork.Repository<Point>()
                .Query()
                .Where(x => x.StudentJoin.GroupId == groupId &&
                    (teacherInstruct.Contains(x.TeacherId) == false && x.Type == Point.TYPE_INSTRUCTION))
                .ToListAsync();
            if (pointIOld != null)
            {
                foreach (var point in pointIOld)
                {
                    await pEvent._unitOfWork.Repository<Point>().DeleteAsync(point);
                    await pEvent._unitOfWork.Save();
                }
            }

            var pointROld = await pEvent._unitOfWork.Repository<Point>()
                .Query()
                .Where(x => x.StudentJoin.GroupId == groupId &&
                    (teacherReview.Contains(x.TeacherId) == false && x.Type == Point.TYPE_REVIEW))
                 .ToListAsync();
            if (pointROld != null)
            {
                foreach (var point in pointROld)
                {
                    await pEvent._unitOfWork.Repository<Point>().DeleteAsync(point);
                    await pEvent._unitOfWork.Save();
                }
            }
        }

    }
}
