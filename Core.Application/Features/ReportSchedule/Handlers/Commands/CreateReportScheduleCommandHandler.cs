using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.DTOs.ReportSchedule.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.ReportSchedule.Events;
using Core.Application.Features.ReportSchedule.Requests.Commands;
using Core.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.ReportSchedule.Handlers.Commands
{
    public class CreateReportScheduleCommandHandler : IRequestHandler<CreateReportScheduleRequest, Result<ReportScheduleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateReportScheduleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ReportScheduleDto>> Handle(CreateReportScheduleRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateReportScheduleDtoValidor(_unitOfWork,
                request.createReportScheduleDto.TimeStart ?? DateTime.Now);
            var validationResult = await validator.ValidateAsync(request.createReportScheduleDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ReportScheduleDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var reportSchedule = _mapper.Map<ReportScheduleEntity>(request.createReportScheduleDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateReportScheduleUpdateReportScheduleEvent(reportSchedule, httpContextAccessor, unitOfWork));

                    }
                });

                var newReportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>().AddAsync(reportSchedule);
                await _unitOfWork.Save(cancellationToken);

                var reportScheduleDto = _mapper.Map<ReportScheduleDto>(newReportSchedule);

                return Result<ReportScheduleDto>.Success(reportScheduleDto, (int)HttpStatusCode.Created);
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
