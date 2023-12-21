using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.Exceptions;
using Core.Application.Features.Points.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Points.Handlers.Queries
{
    public class ListPointOfGroupQueryHandler : IRequestHandler<ListPointOfGroupRequest, PaginatedResult<List<PointDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly IHttpContextAccessor _httpContext;

        public ListPointOfGroupQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }
        public async Task<PaginatedResult<List<PointDto>>> Handle(ListPointOfGroupRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListPointOfGroupValidator(_unitOfWork, request.isGetGroupMe);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<PointDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Point>().GetAllInclude();

            if (request.isGetGroupMe == true)
            {
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;

                if (userType != CLAIMS_VALUES.TYPE_STUDENT)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }
                // Khoa của tôi
                var facultyId = await _unitOfWork.Repository<Student>()
                                               .Query()
                                               .Where(x => x.UserId == int.Parse(userId))
                                               .Include(x => x.Major)
                                               .ThenInclude(x => x.Industry)
                                               .Select(x => x.Major.Industry.FacultyId)
                                               .FirstOrDefaultAsync();

                var periodId = await _unitOfWork.Repository<RegistrationPeriod>()
                                        .Query()
                                        .Where(x => x.FacultyId == facultyId && x.IsActive == true)
                                        .Select(x => x.Id)
                                        .FirstOrDefaultAsync();
                if (periodId == null)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }
                var groupId = await _unitOfWork.Repository<StudentJoin>()
                                .Query()
                                .Where(x => x.Student.UserId == int.Parse(userId) &&
                                            x.RegistrationPeriodId == periodId)
                                .Select(x => x.GroupId)
                                .FirstOrDefaultAsync();
                if (groupId == null)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }

                query = query.Where(x => x.StudentJoin.GroupId == groupId);
            }
            else
            {
                query = query.Where(x => x.StudentJoin.GroupId == request.groupId);
            }    

            int totalCount = await query.CountAsync();

            query = query.Include(x => x.Teacher)
                         .Include(x => x.StudentJoin)
                            .ThenInclude(x => x.Student);

            query = _sieveProcessor.Apply(sieve, query);


            var points = await query.ToListAsync();

            var mapPoints = _mapper.Map<List<PointDto>>(points);
            return PaginatedResult<List<PointDto>>.Success(
                mapPoints, totalCount, request.page,
                request.pageSize);
        }
    }
}
