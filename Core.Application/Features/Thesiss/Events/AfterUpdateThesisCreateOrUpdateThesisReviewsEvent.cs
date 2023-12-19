using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdateThesisReviewsEvent : INotification
    {
        public UpdateThesisDto _updateThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public IHttpContextAccessor _httpContext;

        public AfterUpdateThesisCreateOrUpdateThesisReviewsEvent(UpdateThesisDto updateThesisDto,
            ThesisDto thesisDto, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _updateThesisDto = updateThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }
    }

    public class AfterUpdateThesisUpdateThesisReviewsHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdateThesisReviewsEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdateThesisReviewsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var userId = pEvent._httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = pEvent._httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                return;
            }

            // Từ id của người dùng lấy ra id của giáo viên
            var teacher = await pEvent._unitOfWork.Repository<Teacher>()
                .Query().Include(x => x.HeadDepartment_Department)
                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

            if(teacher.HeadDepartment_Department == null)
            {
                return;
            }
            var departmentTeacherId = await pEvent._unitOfWork.Repository<Department>()
                            .Query()
                            .Where(x => x.Id == pEvent._thesisDto.LecturerThesis.DepartmentId)
                            .Select(x => x.HeadDepartment_TeacherId)
                            .FirstOrDefaultAsync();
            if(departmentTeacherId != teacher.Id)
            {
                return;
            }    
            


            try
            {
                var thesisReviews = await pEvent._unitOfWork.Repository<ThesisReview>()
                                            .GetAllInclude()
                                            .Where(x => x.ThesisId == pEvent._updateThesisDto.Id)
                                            .ToListAsync();

                if (thesisReviews == null)
                {
                    // Create
                    if (pEvent._updateThesisDto.ThesisReviewsId != null)
                    {
                        foreach (int teacherId in pEvent._updateThesisDto.ThesisReviewsId)
                        {
                            ThesisReview thesisReview = new ThesisReview()
                            {
                                TeacherId = teacherId,
                                ThesisId = pEvent._thesisDto.Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisReview>().AddAsync(thesisReview);
                        }
                        await pEvent._unitOfWork.Save();
                    }
                }
                else
                {
                    // Update
                    if (pEvent._updateThesisDto.ThesisReviewsId != null)
                    {
                        List<int?> exceptNewList = thesisReviews.Select(x => x.TeacherId)
                                    .Except(pEvent._updateThesisDto.ThesisReviewsId).ToList();
                        List<int?> exceptOldList = pEvent._updateThesisDto.ThesisReviewsId
                                    .Except(thesisReviews.Select(x => x.TeacherId)).ToList();

                        foreach (int teacherId in exceptNewList)
                        {
                            ThesisReview thesisReview = new ThesisReview()
                            {
                                Id = thesisReviews.FirstOrDefault(x => x.TeacherId == teacherId).Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisReview>().DeleteAsync(thesisReview);
                        }

                        foreach (int teacherId in exceptOldList)
                        {
                            ThesisReview thesisReview = new ThesisReview()
                            {
                                TeacherId = teacherId,
                                ThesisId = pEvent._thesisDto.Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisReview>().AddAsync(thesisReview);
                        }

                        await pEvent._unitOfWork.Save();
                    }
                    else
                    {
                        // Delete all
                        foreach (int teacherId in thesisReviews.Select(x => x.TeacherId))
                        {
                            ThesisReview thesisReview = new ThesisReview()
                            {
                                Id = thesisReviews.FirstOrDefault(x => x.TeacherId == teacherId).Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisReview>().DeleteAsync(thesisReview);
                        }

                        await pEvent._unitOfWork.Save();
                    }    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
