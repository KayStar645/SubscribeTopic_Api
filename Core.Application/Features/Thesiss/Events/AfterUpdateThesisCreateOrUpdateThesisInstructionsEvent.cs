using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Features.Thesiss.Events
{
    public class AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent : INotification
    {
        public UpdateThesisDto _updateThesisDto { get; set; }

        public ThesisDto _thesisDto { get; set; }

        public IUnitOfWork _unitOfWork { get; set; }

        public IHttpContextAccessor _httpContext;

        public AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent(UpdateThesisDto updateThesisDto,
            ThesisDto thesisDto, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _updateThesisDto = updateThesisDto;
            _thesisDto = thesisDto;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
        }
    }

    public class AfterUpdateThesisUpdateThesisInstructionsHandler : INotificationHandler<AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent>
    {
        public async Task Handle(AfterUpdateThesisCreateOrUpdateThesisInstructionsEvent pEvent, CancellationToken cancellationToken)
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

            if (teacher.HeadDepartment_Department == null)
            {
                return;
            }
            var departmentTeacherId = await pEvent._unitOfWork.Repository<Department>()
                            .Query()
                            .Where(x => x.Id == pEvent._thesisDto.LecturerThesis.DepartmentId)
                            .Select(x => x.HeadDepartment_TeacherId)
                            .FirstOrDefaultAsync();
            if (departmentTeacherId != teacher.Id)
            {
                return;
            }

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
