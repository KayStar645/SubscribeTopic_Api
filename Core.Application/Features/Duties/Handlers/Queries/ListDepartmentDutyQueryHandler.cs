using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Exceptions;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Queries
{
    public class ListDepartmentDutyQueryHandler : IRequestHandler<ListDepartmentDutyRequest, PaginatedResult<List<DepartmentDutyDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public IHttpContextAccessor _httpContext;

        public ListDepartmentDutyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }

        public async Task<PaginatedResult<List<DepartmentDutyDto>>> Handle(ListDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListDepartmentDutyValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDutyDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Duty>().GetAllInclude();

            var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
            var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
            if (userType != CLAIMS_VALUES.TYPE_TEACHER)
            {
                throw new UnauthorizedException(StatusCodes.Status403Forbidden);
            }
            // Từ id của người dùng lấy ra id của giáo viên
            var teacher = await _unitOfWork.Repository<Teacher>()
                    .Query().Include(x => x.Department).ThenInclude(x => x.Faculty)
                            .Include(x => x.Dean_Faculty)
                            .Include(x => x.HeadDepartment_Department)
                    .FirstOrDefaultAsync(x => x.UserId == int.Parse(userId));

            // Chỉ lấy khoa của bộ môn mình
            query = query.Where(x => x.Type == Duty.TYPE_DEPARTMENT
                    && x.DepartmentId == teacher.DepartmentId);

            // Không phải trưởng bộ môn thì xét giảng viên
            if (teacher.HeadDepartment_Department == null)
            {
                query = query.Where(x => x.TeacherId == teacher.Id);
            }

            if (request.isAllDetail || request.isGetDepartment == true)
            {
                query = query.Include(x => x.Department);
            }
            if (request.isAllDetail || request.isGetTeacher == true)
            {
                query = query.Include(x => x.Teacher);
            }
            if (request.isAllDetail || request.isGetForDuty == true)
            {
                query = query.Include(x => x.ForDuty);
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var duties = await query.ToListAsync();

            var mapDuties = _mapper.Map<List<DepartmentDutyDto>>(duties);

            return PaginatedResult<List<DepartmentDutyDto>>.Success(
                mapDuties, totalCount, request.page,
                request.pageSize);
        }
    }
}
