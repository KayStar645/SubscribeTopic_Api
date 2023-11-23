using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Major;
using Core.Application.DTOs.Teacher;
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

            if (request.isAllDetail == true || request.isGetIssuer == true)
            {
                query = _unitOfWork.Repository<Thesis>().AddInclude(query, x => x.LecturerThesis);
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

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
