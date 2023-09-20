using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
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
    public class ListStudentRequestHandler : IRequestHandler<ListStudentRequest<StudentDto>, PaginatedResult<List<StudentDto>>>
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

        public async Task<PaginatedResult<List<StudentDto>>> Handle(ListStudentRequest<StudentDto> request, CancellationToken cancellationToken)
        {
            var validator = new ListBaseRequestValidator<StudentDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<StudentDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Student>().GetAllInclude();

            if (request.IsAllDetail)
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

            query = _sieveProcessor.Apply(sieve, query);

            var students = await query.ToListAsync();

            var mapStudents = _mapper.Map<List<StudentDto>>(students);

            return PaginatedResult<List<StudentDto>>.Create(
                mapStudents, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
