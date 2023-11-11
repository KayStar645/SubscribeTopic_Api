using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterCreateThesisCreateThesisMajorsEvent : INotification
    {
        public CreateThesisDto _createThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterCreateThesisCreateThesisMajorsEvent(CreateThesisDto createThesisDto, ThesisDto thesisDto, IUnitOfWork unitOfWork)
        {
            _createThesisDto = createThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterCreateThesisCreateThesisMajorsHandler : INotificationHandler<AfterCreateThesisCreateThesisMajorsEvent>
    {
        public async Task Handle(AfterCreateThesisCreateThesisMajorsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                if (pEvent._createThesisDto.ThesisMajorsId != null)
                {
                    foreach (int majorId in pEvent._createThesisDto.ThesisMajorsId)
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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
