using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.DTOs.ReportSchedule.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.ReportSchedule.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.ReportSchedule.Handlers.Commands
{
    public class UpdateReportScheduleCommandHandler : IRequestHandler<UpdateReportScheduleRequest, Result<ReportScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateReportScheduleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ReportScheduleDto>> Handle(UpdateReportScheduleRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateReportScheduleDtoValidor(_unitOfWork, 
                request.updateReportScheduleDto.TimeStart ?? DateTime.Now);
            var validationResult = await validator.ValidateAsync(request.updateReportScheduleDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ReportScheduleDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findReportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>().GetByIdAsync(request.updateReportScheduleDto.Id);

                if (findReportSchedule is null)
                {
                    return Result<ReportScheduleDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateReportScheduleDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }
                // Lịch loại W: Không được trùng gvhd và sinh viên trong nhóm
                var teacherIdInstruction = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == findReportSchedule.ThesisId)
                                                .Select(x => x.TeacherId)
                                                .ToListAsync();
                var groupId = await _unitOfWork.Repository<ThesisRegistration>()
                                        .Query()
                                        .Where(x => x.ThesisId == findReportSchedule.ThesisId)
                                        .Select(x => x.GroupId)
                                        .FirstOrDefaultAsync();

                var check = await _unitOfWork.Repository<ReportScheduleEntity>()
                                    .Query()
                                    .AnyAsync(x => (x.TimeStart <= request.updateReportScheduleDto.TimeStart && request.updateReportScheduleDto.TimeStart <= x.TimeEnd ||
                                                    x.TimeStart <= request.updateReportScheduleDto.TimeEnd && request.updateReportScheduleDto.TimeEnd <= x.TimeEnd ||
                                                    request.updateReportScheduleDto.TimeStart <= x.TimeStart && x.TimeStart <= request.updateReportScheduleDto.TimeEnd) &&
                                                    (x.Thesis.ThesisInstructions.Any(x => teacherIdInstruction.Contains(x.TeacherId)) ||
                                                     x.Thesis.ThesisRegistration.GroupId == groupId) && x.Id != findReportSchedule.Id);
                if (check)
                {
                    throw new BadRequestException("Thời gian bị trùng với lịch khác!");
                }


                // Lịch loại R: không được trùng gvhd, gvpb và sinh viên trong nhóm
                if (findReportSchedule.Type == ReportScheduleEntity.TYPE_REVIEW)
                {
                    var teacherIdReview = await _unitOfWork.Repository<ThesisReview>()
                                                .Query()
                                                .Where(x => x.ThesisId == findReportSchedule.ThesisId)
                                                .Select(x => x.TeacherId)
                                                .ToListAsync();
                    var check2 = await _unitOfWork.Repository<ReportScheduleEntity>()
                                    .Query()
                                    .AnyAsync(x => (x.TimeStart <= request.updateReportScheduleDto.TimeStart && request.updateReportScheduleDto.TimeStart <= x.TimeEnd ||
                                                    x.TimeStart <= request.updateReportScheduleDto.TimeEnd && request.updateReportScheduleDto.TimeEnd <= x.TimeEnd ||
                                                    request.updateReportScheduleDto.TimeStart <= x.TimeStart && x.TimeStart <= request.updateReportScheduleDto.TimeEnd) &&
                                                    x.Thesis.ThesisReviews.Any(x => teacherIdInstruction.Contains(x.TeacherId)) && x.Id != findReportSchedule.Id);
                    if (check2)
                    {
                        throw new BadRequestException("Thời gian bị trùng với lịch khác!");
                    }
                }

                findReportSchedule.CopyPropertiesFrom(request.updateReportScheduleDto);

                var newReportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>().UpdateAsync(findReportSchedule);
                await _unitOfWork.Save(cancellationToken);

                var ReportScheduleDto = _mapper.Map<ReportScheduleDto>(newReportSchedule);

                return Result<ReportScheduleDto>.Success(ReportScheduleDto, (int)HttpStatusCode.OK);
            }
            catch (NotFoundException ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}