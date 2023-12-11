using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.Features.ReportSchedule.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.ReportSchedule.Handlers.Queries
{
    public class DetailReportScheduleQueryHandler : IRequestHandler<DetailReportScheduleRequest, Result<ReportScheduleDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailReportScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ReportScheduleDto>> Handle(DetailReportScheduleRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ReportScheduleDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<ReportScheduleEntity>().GetByIdInclude(request.id);

                if (request.isAllDetail || request.isGetThesis == true)
                {
                    query = query.Include(x => x.Thesis);
                }
                if (request.isAllDetail || request.isGetTeacher == true)
                {
                    query = query.Include(x => x.Teacher);
                }

                var findReportSchedule = await query.SingleAsync();

                if (findReportSchedule is null)
                {
                    return Result<ReportScheduleDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var ReportScheduleDto = _mapper.Map<ReportScheduleDto>(findReportSchedule);

                return Result<ReportScheduleDto>.Success(ReportScheduleDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<ReportScheduleDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}