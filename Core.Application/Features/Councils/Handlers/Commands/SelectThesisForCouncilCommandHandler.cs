using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Council;
using Core.Application.DTOs.Council.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Councils.Events;
using Core.Application.Features.Councils.Requests.Commands;
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
                    var t = await _unitOfWork.Repository<Thesis>()
                                    .FirstOrDefaultAsync(x => x.Id == thesis.ThesisId);

                    // Kiểm tra đề tài đã được GVHD và GVPB duyệt chưa
                    // Giảng viên hướng dẫn
                    var approveIns = await _unitOfWork.Repository<ThesisInstruction>()
                                            .Query()
                                            .Where(x => x.ThesisId == thesis.ThesisId)
                                            .ToListAsync();

                    // Giảng viên phản biện
                    var approveRev = await _unitOfWork.Repository<ThesisReview>()
                                            .Query()
                                            .Where(x => x.ThesisId == thesis.ThesisId)
                                            .ToListAsync();
                    if(approveIns.Any(x => x.IsApprove == false) || approveRev.Any(x => x.IsApprove == false))
                    {
                        throw new BadRequestException($"Đề tài {t.InternalCode}: Đề tài chưa được duyệt bởi GVHD hoặc GVPB!");
                    }    


                    // Kiểm tra giảng viên trong hội đồng và GVHD & GVPB không được trùng nhau
                    // Giảng viên trong hội đồng
                    var teacherIdCouncil = await _unitOfWork.Repository<Commissioner>()
                                            .Query()
                                            .Where(x => x.CouncilId == request.selectThesisForCouncilDto.CouncilId)
                                            .Select(x => x.TeacherId)
                                            .ToListAsync();

                    // Giảng viên hướng dẫn
                    var teacherIdIns = await _unitOfWork.Repository<ThesisInstruction>()
                                            .Query()
                                            .Where(x => x.ThesisId == thesis.ThesisId)
                                            .Select(x => x.TeacherId)
                                            .ToListAsync();

                    // Giảng viên phản biện
                    var teacherIdRev = await _unitOfWork.Repository<ThesisReview>()
                                            .Query()
                                            .Where(x => x.ThesisId == thesis.ThesisId)
                                            .Select(x => x.TeacherId)
                                            .ToListAsync();

                    if (teacherIdIns.Intersect(teacherIdCouncil).Any() || teacherIdRev.Intersect(teacherIdCouncil).Any())
                    {
                        throw new BadRequestException($"Đề tài {t.InternalCode}: Giảng viên trong hội đồng trùng với GVHD hoặc GVPB không hợp lệ!");
                    }

                    var findThesis = await _unitOfWork.Repository<Thesis>()
                                            .Query()
                                            .Where(x => x.Id == thesis.ThesisId)
                                            .FirstOrDefaultAsync();
                    findThesis.CouncilId = request.selectThesisForCouncilDto.CouncilId;

                    await _unitOfWork.Repository<Thesis>().UpdateAsync(findThesis);
                    await _unitOfWork.Save(cancellationToken);

                    // Kiểm tra lịch của toàn bộ những người tham gia: GVHD, GVPB, thành viên hội đồng, sinh viên
                    var teacherIns = await _unitOfWork.Repository<ThesisInstruction>()
                                        .Query()
                                        .Where(x => x.ThesisId == findThesis.Id)
                                        .Select(x => x.TeacherId)
                                        .ToListAsync();
                    var teacherRev = await _unitOfWork.Repository<ThesisReview>()
                                            .Query()
                                            .Where(x => x.ThesisId == findThesis.Id)
                                            .Select(x => x.TeacherId)
                                            .ToListAsync();
                    var thesisCouncil = await _unitOfWork.Repository<Thesis>()
                                            .Query()
                                            .Where(x => x.CouncilId == council.Id)
                                            .Select(x => x.Id)
                                            .ToListAsync();

                    var check = await _unitOfWork.Repository<Schedule>()
                                .Query()
                                .AnyAsync(x => (x.TimeStart <= thesis.TimeStart && thesis.TimeStart <= x.TimeEnd ||
                                                x.TimeStart <= thesis.TimeEnd && thesis.TimeEnd <= x.TimeEnd ||
                                                thesis.TimeStart <= x.TimeStart && x.TimeStart <= thesis.TimeEnd) &&
                                                (x.Thesis.ThesisInstructions.Any(x => teacherIns.Contains(x.TeacherId)) ||
                                                x.Thesis.ThesisReviews.Any(x => teacherRev.Contains(x.TeacherId)) ||
                                                thesisCouncil.Contains((int)x.ThesisId)));

                    if (check)
                    {
                        throw new BadRequestException("Thời gian bị trùng với lịch khác!");
                    }

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
