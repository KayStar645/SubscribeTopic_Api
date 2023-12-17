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
                var thesisInstructions = await pEvent._unitOfWork.Repository<ThesisInstruction>()
                                            .GetAllInclude()
                                            .Where(x => x.ThesisId == pEvent._updateThesisDto.Id)
                                            .ToListAsync();

                if (thesisInstructions == null)
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
                        await pEvent._unitOfWork.Save();
                    }
                } 
                else
                {
                    // Update
                    if (pEvent._updateThesisDto.ThesisInstructionsId != null)
                    {
                        List<int?> exceptNewList = thesisInstructions.Select(x => x.TeacherId)
                                                            .Except(pEvent._updateThesisDto.ThesisInstructionsId).ToList();
                        List<int?> exceptOldList = pEvent._updateThesisDto.ThesisInstructionsId
                                                            .Except(thesisInstructions.Select(x => x.TeacherId)).ToList();

                        foreach (int? teacherId in exceptNewList)
                        {
                            ThesisInstruction thesisInstruction = new ThesisInstruction()
                            {
                                Id = thesisInstructions.FirstOrDefault(x => x.TeacherId == teacherId).Id
                            };
                            // Để xóa thì cần id của nó
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

                        await pEvent._unitOfWork.Save();
                    }
                    else
                    {
                        // Delete all
                        foreach (int teacherId in thesisInstructions.Select(x => x.TeacherId))
                        {
                            ThesisInstruction thesisInstruction = new ThesisInstruction()
                            {
                                Id = thesisInstructions.FirstOrDefault(x => x.TeacherId == teacherId).Id
                            };
                            await pEvent._unitOfWork.Repository<ThesisInstruction>().DeleteAsync(thesisInstruction);
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
