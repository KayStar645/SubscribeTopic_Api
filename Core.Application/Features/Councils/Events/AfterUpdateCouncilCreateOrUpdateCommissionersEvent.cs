using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Council;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Councils.Events
{
    public class AfterUpdateCouncilCreateOrUpdateCommissionersEvent : INotification
    {
        public UpdateCouncilDto _updateCouncilDto { get; set; }

        public CouncilDto _councilDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateCouncilCreateOrUpdateCommissionersEvent(UpdateCouncilDto updateCouncilDto,
            CouncilDto councilDto, IUnitOfWork unitOfWork)
        {
            _updateCouncilDto = updateCouncilDto;
            _councilDto = councilDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateCouncilCreateOrUpdateCommissionersHandler : INotificationHandler<AfterUpdateCouncilCreateOrUpdateCommissionersEvent>
    {
        public async Task Handle(AfterUpdateCouncilCreateOrUpdateCommissionersEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            var commissioners = await pEvent._unitOfWork.Repository<Commissioner>()
                                            .GetAllInclude()
                                            .Where(x => x.CouncilId == pEvent._updateCouncilDto.Id)
                                            .ToListAsync();

            if (commissioners == null)
            {
                // Create
                if (pEvent._updateCouncilDto.Commissioners != null)
                {
                    foreach (var commissioner in pEvent._updateCouncilDto.Commissioners)
                    {
                        Commissioner com = new Commissioner()
                        {
                            Position = commissioner.Position,
                            TeacherId = commissioner.TeacherId,
                            CouncilId = pEvent._updateCouncilDto.Id,
                        };
                        await pEvent._unitOfWork.Repository<Commissioner>().AddAsync(com);
                    }
                    await pEvent._unitOfWork.Save();
                }
            }
            else
            {
                // Update
                if (pEvent._updateCouncilDto.Commissioners != null)
                {
                    // Delete old
                    foreach (var commissioner in commissioners)
                    {
                        await pEvent._unitOfWork.Repository<Commissioner>().DeleteAsync(commissioner);
                    }
                    await pEvent._unitOfWork.Save();

                    // Create new
                    foreach (var commissioner in pEvent._updateCouncilDto.Commissioners)
                    {
                        Commissioner com = new Commissioner
                        {
                            Position = commissioner.Position,
                            TeacherId = commissioner.TeacherId,
                            CouncilId = pEvent._councilDto.Id,
                        };
                        await pEvent._unitOfWork.Repository<Commissioner>().AddAsync(com);
                    }
                    await pEvent._unitOfWork.Save();
                }
            }
        }

    }
}
