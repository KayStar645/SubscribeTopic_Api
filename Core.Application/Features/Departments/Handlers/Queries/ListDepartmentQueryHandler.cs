using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Department;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Core.Application.Features.Departments.Handlers.Queries
{
    public class ListDepartmentQueryHandler : IRequestHandler<ListDepartmentRequest, PaginatedResult<List<DepartmentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListDepartmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<DepartmentDto>>> Handle(ListDepartmentRequest request, CancellationToken cancellationToken)
        {
            var validator = new DepartmentDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var validationContext = new ValidationContext(request, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (isValid == false)
            {
                var errorMessages = validationResults.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Department>().GetAllInclude()
                                   .Where(x => x.FacultyId == request.facultyId);

            if (request.isAllDetail)
            {
                query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.HeadDepartment_Teacher);
            }
            else
            {
                if (request.isGetFaculty == true)
                {
                    query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                }

                if (request.isGetHeadDepartment == true)
                {
                    query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.HeadDepartment_Teacher);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var departments = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<DepartmentDto>>(departments);
            return PaginatedResult<List<DepartmentDto>>.Success(
                mapTeachers, totalCount, request.page,
                request.pageSize);
        }
    }
}
