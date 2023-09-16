using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Department;
using Core.Application.Features.Base.Requests.Queries;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Departments.Handlers.Queries
{
    public class ListDepartmentRequestHandler : IRequestHandler<ListDepartmentRequest<DepartmentDto>, PaginatedResult<List<DepartmentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListDepartmentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }
        public async Task<PaginatedResult<List<DepartmentDto>>> Handle(ListDepartmentRequest<DepartmentDto> request, CancellationToken cancellationToken)
        {
            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Department>().GetAllInclude();

            if (request.IsAllDetail)
            {
                query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.HeadDepartment_Teacher);
            }
            else
            {
                if (request.IsGetFaculty == true)
                {
                    query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                }

                if (request.IsGetHeadDepartment == true)
                {
                    query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.HeadDepartment_Teacher);
                }
            }

            query = _sieveProcessor.Apply(sieve, query);

            var departments = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<DepartmentDto>>(departments);
            return PaginatedResult<List<DepartmentDto>>.Create(
                mapTeachers, 0, request.Page,
                request.PageSize, (int)HttpStatusCode.OK);
        }
    }
}
