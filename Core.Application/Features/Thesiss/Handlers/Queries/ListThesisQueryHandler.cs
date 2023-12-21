using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
using Core.Application.DTOs.Thesis;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisQueryHandler : IRequestHandler<ListThesisRequest, PaginatedResult<List<ThesisDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContext;

        public ListThesisQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }

        public async Task<PaginatedResult<List<ThesisDto>>> Handle(ListThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListThesisDtoValidator(_unitOfWork, request.departmentId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ThesisDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Thesis>().GetAllInclude();

            // Chỉ lấy đề tài mà giảng viên đang truy cập hướng dẫn
            var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            var teacher = await _unitOfWork.Repository<Teacher>()
                .Query().Include(x => x.HeadDepartment_Department)
                        .Include(x => x.Dean_Faculty)
                .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));
            if(request.filters == string.Empty)
            {
                if (teacher.Dean_Faculty != null)
                {
                    query = query.Where(x => x.LecturerThesis.UserId == int.Parse(userId) ||
                                x.ThesisInstructions.Any(x => x.Teacher.UserId == int.Parse(userId)) ||
                                x.ThesisReviews.Any(x => x.Teacher.UserId == int.Parse(userId)) ||
                                (x.Duty.ForDuty.FacultyId == teacher.Dean_Faculty.Id && x.Status == Thesis.STATUS_APPROVED));
                }
                else if (teacher.HeadDepartment_Department != null)
                {
                    query = query.Where(x => x.LecturerThesis.UserId == int.Parse(userId) ||
                                x.ThesisInstructions.Any(x => x.Teacher.UserId == int.Parse(userId)) ||
                                x.ThesisReviews.Any(x => x.Teacher.UserId == int.Parse(userId)) ||
                                (x.Duty.DepartmentId == teacher.HeadDepartment_Department.Id &&
                                x.Status == Thesis.STATUS_APPROVED || x.Status == Thesis.STATUS_APPROVE_REQUEST));
                }
                else
                {
                    // Đề tài mình ra/hd/pb
                    query = query.Where(x => x.LecturerThesis.UserId == int.Parse(userId) ||
                                        x.ThesisInstructions.Any(x => x.Teacher.UserId == int.Parse(userId)) ||
                                        x.ThesisReviews.Any(x => x.Teacher.UserId == int.Parse(userId)));
                }
            }    

            if (request.departmentId != null)
            {
                //query = query.Where(x => x.Duty.DepartmentId == request.departmentId);
                query = query.Where(x => x.LecturerThesis.DepartmentId == request.departmentId);
            }
            else if (request.facultyId != null)
            {
                //query = query.Where(x => x.Duty.ForDuty.FacultyId == request.facultyId);
                query = query.Where(x => x.LecturerThesis.Department.FacultyId == request.facultyId);
            }

            if (request.isAllDetail == true || request.isGetIssuer == true)
            {
                query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var thesiss = await query.ToListAsync();

            var mapThesiss = _mapper.Map<List<ThesisDto>>(thesiss);

            foreach (var thesis in mapThesiss)
            {
                if (request.isAllDetail == true || request.isGetThesisInstructions == true)
                {
                    var thesisInstructions = await _unitOfWork.Repository<ThesisInstruction>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesis.ThesisInstructions = _mapper.Map<List<TeacherDto>>(thesisInstructions);
                }

                if (request.isAllDetail == true || request.isGetThesisReviews == true)
                {
                    var thesisReviews = await _unitOfWork.Repository<ThesisReview>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesis.Id)
                                                .Include(x => x.Teacher)
                                                .Select(x => x.Teacher)
                                                .ToListAsync();

                    thesis.ThesisReviews = _mapper.Map<List<TeacherDto>>(thesisReviews);
                }

                if (request.isAllDetail == true || request.isGetThesisMajors == true)
                {
                    var thesisMajors = await _unitOfWork.Repository<ThesisMajor>()
                                                .Query()
                                                .Where(x => x.ThesisId == thesis.Id)
                                                .Include(x => x.Major)
                                                .Select(x => x.Major)
                                                .ToListAsync();

                    thesis.ThesisMajors = _mapper.Map<List<MajorDto>>(thesisMajors);
                }
            }    

            return PaginatedResult<List<ThesisDto>>.Success(
                mapThesiss, totalCount, request.page,
                request.pageSize);
        }
    }
}
