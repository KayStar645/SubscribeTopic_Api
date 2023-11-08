using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.Thesiss.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Queries
{
    public class ListThesisRequestHandler : IRequestHandler<ListThesisRequest, PaginatedResult<List<ThesisDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListThesisRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
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

            if (request.departmentId != null)
            {
                query = query.Where(x => x.LecturerThesis.DepartmentId == request.departmentId);
            }
            else if (request.facultyId != null)
            {
                query = query.Where(x => x.LecturerThesis.Department.FacultyId == request.facultyId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
                query = query.Include(x => x.ThesisInstructions);
                query = query.Include(x => x.ThesisReviews);
                query = query.Include(x => x.ThesisMajors);
            }
            else
            {
                if (request.isGetIssuer == true)
                {
                    query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
                }

                if (request.isGetThesisInstructions == true)
                {
                    query = query.Include(x => x.ThesisInstructions);
                }

                if (request.isGetThesisReviews == true)
                {
                    query = query.Include(x => x.ThesisReviews);
                }

                if (request.isGetThesisMajors == true)
                {
                    query = query.Include(x => x.ThesisMajors);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var thesiss = await query.ToListAsync();

            var mapThesiss = _mapper.Map<List<ThesisDto>>(thesiss);

            return PaginatedResult<List<ThesisDto>>.Success(
                mapThesiss, totalCount, request.page,
                request.pageSize);
        }
    }
}
