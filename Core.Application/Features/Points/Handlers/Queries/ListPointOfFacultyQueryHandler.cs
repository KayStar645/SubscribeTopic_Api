using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Point;
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
    public class ListPointOfFacultyQueryHandler : IRequestHandler<ListPointOfFacultyRequest, PaginatedResult<List<PointDto>>>
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
        public async Task<PaginatedResult<List<PointDto>>> Handle(ListPointOfFacultyRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListPointOfFacultyValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<PointDto>>
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

            var Points = await query.ToListAsync();

            var mapPoints = _mapper.Map<List<PointDto>>(Points);
            return PaginatedResult<List<PointDto>>.Success(
                mapPoints, totalCount, request.page,
                request.pageSize);
        }
    }
}
