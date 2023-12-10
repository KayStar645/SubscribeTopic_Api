using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.DTOs.ReportSchedule.Validators;
using Core.Application.Features.ReportSchedule.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using MediatR;
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
            var validator = new UpdateReportScheduleDtoValidor(_unitOfWork);
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

                findReportSchedule.CopyPropertiesFrom(request.updateReportScheduleDto);

                var newReportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>().UpdateAsync(findReportSchedule);
                await _unitOfWork.Save(cancellationToken);

                var ReportScheduleDto = _mapper.Map<ReportScheduleDto>(newReportSchedule);

                return Result<ReportScheduleDto>.Success(ReportScheduleDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}