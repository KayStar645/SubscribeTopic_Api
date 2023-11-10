using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdateThesisMajorsEvent : INotification
    {
        public UpdateThesisDto _updateThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateThesisCreateOrUpdateThesisMajorsEvent(UpdateThesisDto updateThesisDto, ThesisDto thesisDto, IUnitOfWork unitOfWork)
        {
            _updateThesisDto = updateThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateThesisUpdateThesisMajorsHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdateThesisMajorsEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdateThesisMajorsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var thesisMajors = await pEvent._unitOfWork.Repository<ThesisMajor>()
                                            .GetAllInclude()
                                            .Where(x => x.ThesisId == pEvent._updateThesisDto.Id)
                                            .ToListAsync();

                if (thesisMajors == null)
                {
                    // Create
                    if (pEvent._updateThesisDto.ThesisMajorsId != null)
                    {
                        foreach (int majorId in pEvent._updateThesisDto.ThesisMajorsId)
                        {
                            ThesisMajor thesisMajor = new ThesisMajor()
                            {
                                MajorId = majorId,
                                ThesisId = pEvent._thesisDto.Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisMajor>().AddAsync(thesisMajor);
                        }
                        await pEvent._unitOfWork.Save(cancellationToken);
                    }
                }
                else
                {
                    // Update
                    if (pEvent._updateThesisDto.ThesisMajorsId != null)
                    {
                        List<int?> exceptNewList = thesisMajors.Select(x => x.MajorId)
                                        .Except(pEvent._updateThesisDto.ThesisMajorsId).ToList();
                        List<int?> exceptOldList = pEvent._updateThesisDto.ThesisMajorsId
                                        .Except(thesisMajors.Select(x => x.MajorId)).ToList();

                        foreach (int majorId in exceptNewList)
                        {
                            ThesisMajor thesisMajor = new ThesisMajor()
                            {
                               Id = thesisMajors.FirstOrDefault(x => x.MajorId == majorId).Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisMajor>().DeleteAsync(thesisMajor);
                        }

                        foreach (int majorId in exceptOldList)
                        {
                            ThesisMajor thesisMajor = new ThesisMajor()
                            {
                                MajorId = majorId,
                                ThesisId = pEvent._thesisDto.Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisMajor>().AddAsync(thesisMajor);
                        }

                        await pEvent._unitOfWork.Save(cancellationToken);
                    }
                    else
                    {
                        // Delete all
                        foreach (int majorId in thesisMajors.Select(x => x.MajorId))
                        {
                            ThesisMajor thesisMajor = new ThesisMajor()
                            {
                                Id = thesisMajors.FirstOrDefault(x => x.MajorId == majorId).Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisMajor>().DeleteAsync(thesisMajor);
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
