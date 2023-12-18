﻿using Core.Application.Contracts.Persistence;
using Core.Application.Interfaces.Repositories;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Points.Events
{
    public class AfterUpdatePointUpdateStudentJoinEvent : INotification
    {
        public Point _point { get; set; }
        public IUserRepository _userRepository { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdatePointUpdateStudentJoinEvent(Point point, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _point = point;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdatePointUpdateStudentJoinHandler : INotificationHandler<AfterUpdatePointUpdateStudentJoinEvent>
    {
        public async Task Handle(AfterUpdatePointUpdateStudentJoinEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            // Tính lại điểm TB của sinh viên này
            var pointInstruction = await pEvent._unitOfWork.Repository<Point>()
                                    .Query()
                                    .Where(x => x.StudentJoinId == pEvent._point.StudentJoinId &&
                                                x.Type == Point.TYPE_INSTRUCTION)
                                    .AverageAsync(x => x.Scores);

            var pointReview = await pEvent._unitOfWork.Repository<Point>()
                                .Query()
                                .Where(x => x.StudentJoinId == pEvent._point.StudentJoinId &&
                                            x.Type == Point.TYPE_REVIEW)
                                .AverageAsync(x => x.Scores);

            var studentJoin = await pEvent._unitOfWork.Repository<StudentJoin>()
                                .FirstOrDefaultAsync(x => x.Id == pEvent._point.StudentJoinId);
            studentJoin.Score = (pointInstruction ?? 0 + pointReview ?? 0) / 2;
            await pEvent._unitOfWork.Repository<StudentJoin>().UpdateAsync(studentJoin);
            await pEvent._unitOfWork.Save();
        }

    }
}
