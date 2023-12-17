using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.DTOs.StudentJoin;
using Core.Application.DTOs.Teacher;
using Core.Application.Features.Points.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
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

        public ListPointOfThesisQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<ThesisPointDto>>> Handle(ListPointOfThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListPointOfThesisValidator(_unitOfWork, request.isGetThesisCurrentMe);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ThesisPointDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Point>().GetAllInclude();

            query = query.Where(x => x.StudentJoin.Group.ThesisRegistration.ThesisId == request.thesisId);

            query = query.Include(x => x.Teacher)
                         .Include(x => x.StudentJoin)
                            .ThenInclude(x => x.Student);

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var points = await query.ToListAsync();

            // Hậu xử xý
            var pointsByStudent = points.GroupBy(x => x.StudentJoin.Id);
            var thesisPointDtos = new List<ThesisPointDto>();
            foreach (var studentGroup in pointsByStudent)
            {
                var teacherPointDtos = new List<TeacherPointDto>();

                foreach(var point in studentGroup)
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
                    AverageScore = studentGroup.FirstOrDefault()?.StudentJoin?.Score ?? 0,
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
