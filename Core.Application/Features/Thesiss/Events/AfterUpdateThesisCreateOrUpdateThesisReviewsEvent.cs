using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdateThesisReviewsEvent : INotification
    {
        public UpdateThesisDto _updateThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateThesisCreateOrUpdateThesisReviewsEvent(UpdateThesisDto updateThesisDto, ThesisDto thesisDto, IUnitOfWork unitOfWork)
        {
            _updateThesisDto = updateThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateThesisUpdateThesisReviewsHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdateThesisReviewsEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdateThesisReviewsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

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
                        await pEvent._unitOfWork.Save(cancellationToken);
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

                        await pEvent._unitOfWork.Save(cancellationToken);
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

                        await pEvent._unitOfWork.Save(cancellationToken);
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
