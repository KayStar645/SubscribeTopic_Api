using AutoMapper;
using Core.Application.Constants;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
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
    public class ListPointOfThesisQueryHandler : IRequestHandler<ListPointOfThesisRequest, PaginatedResult<List<ThesisPointDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        private readonly IHttpContextAccessor _httpContext;

        public ListPointOfThesisQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            ISieveProcessor sieveProcessor, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _httpContext = httpContext;
        }
        public async Task<PaginatedResult<List<ThesisPointDto>>> Handle(ListPointOfThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListPointOfThesisValidator(_unitOfWork, request.isGetPointMe);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ThesisPointDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Point>().GetAllInclude();

            if(request.isGetPointMe == true)
            {
                var userId = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Uid)?.Value;
                var userType = _httpContext.HttpContext.User.FindFirst(CONSTANT_CLAIM_TYPES.Type)?.Value;
                if(userType != CLAIMS_VALUES.TYPE_STUDENT)
                {
                    throw new UnauthorizedException(StatusCodes.Status403Forbidden);
                }    

                query = query.Where(x => x.StudentJoin.Student.UserId == int.Parse(userId));
            }   
            else
            {
                query = query.Where(x => x.StudentJoin.Group.ThesisRegistration.ThesisId == request.thesisId);
            }    


            query = query.Include(x => x.Teacher)
                         .Include(x => x.StudentJoin)
                            .ThenInclude(x => x.Student);

            query = _sieveProcessor.Apply(sieve, query);

            var points = await query.ToListAsync();

            // Hậu xử xý
            var pointsByStudent = points.GroupBy(x => x.StudentJoin.Id);
            int totalCount = pointsByStudent.Count();
            var thesisPointDtos = new List<ThesisPointDto>();
            foreach (var studentGroup in pointsByStudent)
            {
                var teacherPointDtos = new List<TeacherPointDto>();
                foreach (var point in studentGroup)
                {
                    var teacherPoint = new TeacherPointDto
                    {
                        PointId = point.Id,
                        Type = point.Type,
                        Score = point.Scores,
                        Teacher = _mapper.Map<TeacherDto>(point.Teacher),
                    };
                    teacherPointDtos.Add(teacherPoint);  
                }
                var studentJoin = _mapper.Map<StudentJoinDto>(studentGroup.FirstOrDefault()?.StudentJoin);
                var thesisPointDto = new ThesisPointDto
                {
                    StudentJoinId = studentJoin.Id,
                    Scores = teacherPointDtos,
                    AverageScore = (teacherPointDtos.Where(x => x.Type == Point.TYPE_INSTRUCTION).Average(x => x.Score) +
                                    teacherPointDtos.Where(x => x.Type == Point.TYPE_REVIEW).Average(x => x.Score)) /2,
                    StudentJoin = studentJoin
                };
                thesisPointDtos.Add(thesisPointDto);
            }

            return PaginatedResult<List<ThesisPointDto>>.Success(
                thesisPointDtos, totalCount, request.page,
                request.pageSize);
        }
    }
}
