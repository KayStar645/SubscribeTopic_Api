using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterCreateThesisCreateThesisInstructionsEvent : INotification
    {
        public CreateThesisDto _createThesisDto { get; set; }
        
        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterCreateThesisCreateThesisInstructionsEvent(CreateThesisDto createThesisDto, ThesisDto thesisDto, IUnitOfWork unitOfWork)
        {
            _createThesisDto = createThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterCreateThesisCreateThesisInstructionsHandler : INotificationHandler<AfterCreateThesisCreateThesisInstructionsEvent>
    {
        public async Task Handle(AfterCreateThesisCreateThesisInstructionsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                if(pEvent._createThesisDto.ThesisInstructionsId != null)
                {
                    foreach (int teacherId in pEvent._createThesisDto.ThesisInstructionsId)
                    {
                        ThesisInstruction thesisInstruction = new ThesisInstruction()
                        {
                            TeacherId = teacherId,
                            ThesisId = pEvent._thesisDto.Id
                        };
                        await pEvent._unitOfWork.Repository<ThesisInstruction>().AddAsync(thesisInstruction);
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
