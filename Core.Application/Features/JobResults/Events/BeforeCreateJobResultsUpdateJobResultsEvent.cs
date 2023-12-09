using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using JobResultsEntity = Core.Domain.Entities.JobResults;

namespace Core.Application.Features.JobResults.Events
{
    public class BeforeCreateJobResultsUpdateJobResultsEvent : INotification
    {
        public JobResultsEntity _jobResults { get; set; }

        public IHttpContextAccessor _httpContextAccessor;

        public IUnitOfWork _unitOfWork;

        public BeforeCreateJobResultsUpdateJobResultsEvent(JobResultsEntity jobResults,
            IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _jobResults = jobResults;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
    }

    public class BeforeCreateJobResultsUpdateJobResultsHandler : INotificationHandler<BeforeCreateJobResultsUpdateJobResultsEvent>
    {
        public async Task Handle(BeforeCreateJobResultsUpdateJobResultsEvent pEvent, CancellationToken cancellationToken)
        {
            var userId = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_STUDENT)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            // Phải là đề tài của nhóm mình


            // Phải chưa hết thời gian quy định
            var existTime = await pEvent._unitOfWork.Repository<Job>()
                                    .Query()
                                    .AnyAsync(x => DateTime.Now <= x.Due && x.Id == pEvent._jobResults.JobId);
            if (existTime == false)
            {
                throw new BadRequestException("Quá thời gian nộp bài!");
            }

            // Từ id của người dùng lấy ra id của sinh viên
            var student = await pEvent._unitOfWork.Repository<Student>()
                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

            pEvent._jobResults.StudentId = student.Id;

        }
    }

}