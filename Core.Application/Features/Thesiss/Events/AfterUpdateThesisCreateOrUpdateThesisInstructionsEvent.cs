using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent : INotification
    {
        public UpdateThesisDto _updateThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent(UpdateThesisDto updateThesisDto, ThesisDto thesisDto, IUnitOfWork unitOfWork)
        {
            _updateThesisDto = updateThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
        }
    }

    public class AfterUpdateThesisUpdateThesisInstructionsHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent pEvent, CancellationToken cancellationToken)
        {
            await Task.Yield();

            try
            {
                var thesisInstructionsId = await pEvent._unitOfWork.Repository<ThesisInstruction>()
                    .GetAllInclude(x => x.ThesisId == pEvent._updateThesisDto.Id).Select(x => x.TeacherId).ToListAsync();

                if(thesisInstructionsId == null)
                {
                    // Create
                    if (pEvent._updateThesisDto.ThesisInstructionsId != null)
                    {
                        foreach (int teacherId in pEvent._updateThesisDto.ThesisInstructionsId)
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
                else
                {
                    // Update
                    if (pEvent._updateThesisDto.ThesisInstructionsId != null)
                    {
                        List<int?> exceptNewList = thesisInstructionsId.Except(pEvent._updateThesisDto.ThesisInstructionsId).ToList();
                        List<int?> exceptOldList = pEvent._updateThesisDto.ThesisInstructionsId.Except(thesisInstructionsId).ToList();

                        foreach (int teacherId in exceptNewList)
                        {
                            ThesisInstruction thesisInstruction = new ThesisInstruction()
                            {
                                TeacherId = teacherId,
                                ThesisId = pEvent._thesisDto.Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisInstruction>().DeleteAsync(thesisInstruction);
                        }

                        foreach (int teacherId in exceptOldList)
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
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
