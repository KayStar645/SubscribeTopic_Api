using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ReportSchedule;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.ReportSchedule.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;
using System.Net.WebSockets;
using ReportScheduleEntity = Core.Domain.Entities.ReportSchedule;

namespace Core.Application.Features.ReportSchedule.Handlers.Queries
{
    public class ListReportScheduleQueryHandler : IRequestHandler<ListReportScheduleRequest, PaginatedResult<List<ReportScheduleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContextAccessor;

        public ListReportScheduleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedResult<List<ReportScheduleDto>>> Handle(ListReportScheduleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var validator = new ListReportScheduleDtoValidator(_unitOfWork);
                var result = await validator.ValidateAsync(request);

                if (result.IsValid == false)
                {
                    var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return PaginatedResult<List<ReportScheduleDto>>
                        .Failure((int)HttpStatusCode.BadRequest, errorMessages);
                }

                var sieve = _mapper.Map<SieveModel>(request);

                var query = _unitOfWork.Repository<ReportScheduleEntity>().GetAllInclude();

                var userId = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = _httpContextAccessor.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType == CLAIMS_VALUES.TYPE_TEACHER)
                {
                    // Lấy danh sách lịch của giảng viên
                    var teacher = await _unitOfWork.Repository<Teacher>()
                        .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));
                    var reportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>()
                                                .Query()
                                                .Where(x => x.Thesis.ThesisInstructions
                                                    .Any(x => x.TeacherId == teacher.Id) ||
                                                            x.Thesis.ThesisReviews
                                                     .Any(x => x.TeacherId == teacher.Id))
                                                .ToListAsync();
                }
                else if (userType == CLAIMS_VALUES.TYPE_STUDENT)
                {
                    // Lấy danh sách lịch của sinh viên
                    var student = await _unitOfWork.Repository<Student>()
                                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));
                    var reportSchedule = await _unitOfWork.Repository<ReportScheduleEntity>()
                                                .Query()
                                                .Where(x => x.Thesis.ThesisRegistration.Group.Members
                                                    .Any(x => x.Student.UserId == int.Parse(userId)))
                                                .ToListAsync();
                }
                else
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                if (request.isAllDetail || request.isGetThesis == true)
                {
                    query = query.Include(x => x.Thesis);
                }
                if (request.isAllDetail || request.isGetTeacher == true)
                {
                    query = query.Include(x => x.Teacher);
                }

                query = _sieveProcessor.Apply(sieve, query);

                int totalCount = await query.CountAsync();

                var reportSchedules = await query.ToListAsync();

                var mapReportSchedules = _mapper.Map<List<ReportScheduleDto>>(reportSchedules);

                return PaginatedResult<List<ReportScheduleDto>>.Success(
                    mapReportSchedules, totalCount, request.page,
                    request.pageSize);
            }
            catch (NotFoundException ex)
            {
                return PaginatedResult<List<ReportScheduleDto>>.Failure((int)HttpStatusCode.NotFound, new List<string> { ex.Message });
            }
            catch (BadRequestException ex)
            {
                return PaginatedResult<List<ReportScheduleDto>>.Failure((int)HttpStatusCode.BadRequest, new List<string> { ex.Message });
            }
            catch (UnauthorizedException ex)
            {
                return PaginatedResult<List<ReportScheduleDto>>.Failure(ex.ErrorCode, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ReportScheduleDto>>.Failure((int)HttpStatusCode.InternalServerError, new List<string> { ex.Message });
            }
        }
    }
}
