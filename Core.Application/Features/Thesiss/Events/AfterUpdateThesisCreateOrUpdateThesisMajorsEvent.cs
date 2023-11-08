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
                var thesisMajorsId = await pEvent._unitOfWork.Repository<ThesisMajor>()
                    .GetAllInclude(x => x.ThesisId == pEvent._updateThesisDto.Id).Select(x => x.MajorId).ToListAsync();

                if (thesisMajorsId == null)
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
                        List<int?> exceptNewList = thesisMajorsId.Except(pEvent._updateThesisDto.ThesisMajorsId).ToList();
                        List<int?> exceptOldList = pEvent._updateThesisDto.ThesisMajorsId.Except(thesisMajorsId).ToList();

                        foreach (int majorId in exceptNewList)
                        {
                            ThesisMajor thesisMajor = new ThesisMajor()
                            {
                                MajorId = majorId,
                                ThesisId = pEvent._thesisDto.Id
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
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
