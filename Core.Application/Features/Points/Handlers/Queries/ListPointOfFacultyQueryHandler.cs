using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
using Core.Application.DTOs.StudentJoin;
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
    public class ListPointOfFacultyQueryHandler : IRequestHandler<ListPointOfFacultyRequest, PaginatedResult<List<ListPointDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListPointOfFacultyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<ListPointDto>>> Handle(ListPointOfFacultyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListPointOfFacultyValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<ListPointDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Point>().GetAllInclude();

            query = query.Where(x => x.StudentJoin.Student.Major.Industry.FacultyId == request.facultyId);

            query = query.Include(x => x.Teacher)
                         .Include(x => x.StudentJoin)
                            .ThenInclude(x => x.Student);

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var points = await query.ToListAsync();

            // Hậu xử xý
            var pointsByStudent = points.GroupBy(x => x.StudentJoin.Id);
            var listPointDtos = new List<ListPointDto>();
            foreach (var studentGroup in pointsByStudent)
            {
                var instructionAverage = studentGroup
                    .Where(x => x.Type == Point.TYPE_INSTRUCTION)
                    .Average(x => x.Scores) ?? 0;

                var reviewAverage = studentGroup
                    .Where(x => x.Type == Point.TYPE_REVIEW)
                    .Average(x => x.Scores) ?? 0;

                var councilAverage = studentGroup
                    .Where(x => x.Type == Point.TYPE_COUNCIL)
                    .Average(x => x.Scores) ?? 0;

                var listPointDto = new ListPointDto
                {
                    InstructionScore = instructionAverage,
                    ViewScore = reviewAverage,
                    CouncilScore = councilAverage,
                    AverageScore = double.Parse(((instructionAverage + reviewAverage +councilAverage) / 3).ToString("F2")),
                    StudentJoin = _mapper.Map<StudentJoinDto>(studentGroup.FirstOrDefault()?.StudentJoin)
                };

                listPointDtos.Add(listPointDto);
            }

            return PaginatedResult<List<ListPointDto>>.Success(
                listPointDtos, totalCount, request.page,
                request.pageSize);
        }
    }
}
