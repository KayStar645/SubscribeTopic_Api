using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Council;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Councils.Events
{
    public class AfterCreateCouncilCreateCommissionersEvent : INotification
    {
        public CreateCouncilDto _createCouncilDto { get; set; }

        public CouncilDto _councilDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterCreateCouncilCreateCommissionersEvent(CreateCouncilDto createCouncilDto,
            CouncilDto councilDto, IUnitOfWork unitOfWork)
        {
            _createCouncilDto = createCouncilDto;
            _councilDto = councilDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterCreateCouncilCreateCommissionersHandler : INotificationHandler<AfterCreateCouncilCreateCommissionersEvent>
    {
        public async Task Handle(AfterCreateCouncilCreateCommissionersEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            if(pEvent._createCouncilDto.Commissioners != null)
            {
                foreach(var commissioner in pEvent._createCouncilDto.Commissioners)
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
