using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Council;
using Core.Application.DTOs.Council.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Councils.Events;
using Core.Application.Features.Councils.Requests.Commands;
using Core.Application.Features.Thesiss.Events;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Schedule = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.Councils.Handlers.Commands
{
    public class SelectThesisForCouncilCommandHandler : IRequestHandler<SelectThesisForCouncilRequest, Result<CouncilDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IServiceProvider _serviceProvider;

        public SelectThesisForCouncilCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContext, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<CouncilDto>> Handle(SelectThesisForCouncilRequest request, CancellationToken cancellationToken)
        {
            var validator = new SelectThesisForCouncilDtoValidator(_unitOfWork);
            var validatorResult = await validator.ValidateAsync(request.selectThesisForCouncilDto);

            if (validatorResult.IsValid == false)
            {
                var errorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<CouncilDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType != CLAIMS_VALUES.TYPE_TEACHER)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                var teacher = await _unitOfWork.Repository<Teacher>()
                    .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

                var council = await _unitOfWork.Repository<Council>()
                        .FirstOrDefaultAsync(x => x.Id == request.selectThesisForCouncilDto.CouncilId);

                foreach (var thesis in request.selectThesisForCouncilDto.ListThesis)
                {
                    var findThesis = await _unitOfWork.Repository<Thesis>()
                                            .Query()
                                            .Where(x => x.Id == thesis.ThesisId)
                                            .FirstOrDefaultAsync();
                    findThesis.CouncilId = request.selectThesisForCouncilDto.CouncilId;

                    await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                    await _unitOfWork.Save(cancellationToken);

                    // Tạo lịch cho đề tài này
                    Schedule schedule = new Schedule
                    {
                        TimeStart = thesis.TimeStart,
                        TimeEnd = thesis.TimeEnd,
                        Type = Schedule.TYPE_COUNCIL,
                        Location = council.Location,
                        ThesisId = thesis.ThesisId,
                        TeacherId = teacher.Id,
                    };
                    await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
                    await _unitOfWork.Save(cancellationToken);
                }

                var findCouncil = await _unitOfWork.Repository<Council>()
                        .FirstOrDefaultAsync(x => x.Id == request.selectThesisForCouncilDto.CouncilId);
                

                var councilDto = _mapper.Map<CouncilDto>(findCouncil);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

                        await mediator.Publish(new AfterSelectThesisForCouncilCreateOrUpdatePointEvent(council, request.selectThesisForCouncilDto.ListThesis, unitOfWork));

                    }
                });

                return Result<CouncilDto>.Success(councilDto, (int)HttpStatusCode.OK);
            }
            catch (NotFoundException ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<CouncilDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
