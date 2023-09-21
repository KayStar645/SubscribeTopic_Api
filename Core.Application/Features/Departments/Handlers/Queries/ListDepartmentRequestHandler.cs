﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
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
            var validator = new ListBaseRequestValidator<DepartmentDto>();
            var result = validator.Validate(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<DepartmentDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Department>().GetAllInclude();

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
            int totalCount = await query.CountAsync();

            query = _sieveProcessor.Apply(sieve, query);

            var departments = await query.ToListAsync();

            var mapTeachers = _mapper.Map<List<DepartmentDto>>(departments);
            return PaginatedResult<List<DepartmentDto>>.Success(
                mapTeachers, totalCount, request.page,
                request.pageSize);
        }
    }
}
