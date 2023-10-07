using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Student;
using Core.Application.Features.Students.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Students.Handlers.Queries
{
    public class ListStudentRequestHandler : IRequestHandler<ListStudentRequest, PaginatedResult<List<StudentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        public ListStudentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<StudentDto>>> Handle(ListStudentRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListStudentDtoValidator(_unitOfWork, request.majorId, request.industryId);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<StudentDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Student>().GetAllInclude();

            if(request.majorId != null)
            {
                query = query.Where(x => x.MajorId == request.majorId);
            }   
            else if (request.industryId != null)
            {
                query = query.Where(x => x.Major.IndustryId == request.industryId);
            }   
            else
            {
                query = query.Where(x => x.Major.Industry.FacultyId == request.facultyId);
            }

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
            }
            else
            {
                if (request.isGetMajor == true)
                {
                    query = _unitOfWork.Repository<Student>().AddInclude(query, x => x.Major);
                }
            }

            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var students = await query.ToListAsync();

            var mapStudents = _mapper.Map<List<StudentDto>>(students);

            return PaginatedResult<List<StudentDto>>.Success(
                mapStudents, totalCount, request.page,
                request.pageSize);
        }
    }
}
