﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Department;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Departments.Handlers.Queries
{
    public class DetailDepartmentRequestHandler : IRequestHandler<DetailDepartmentRequest, Result<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DetailDepartmentRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> Handle(DetailDepartmentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<Department>().GetByIdInclude(request.Id);

                if (request.IsAllDetail)
                {
                    query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.IsGetFaculty == true)
                    {
                        query = _unitOfWork.Repository<Department>().AddInclude(query, x => x.Faculty);
                    }
                }

                var findDepartment = await query.SingleAsync();

                if (findDepartment is null)
                {
                    return Result<DepartmentDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var departmentDto = _mapper.Map<DepartmentDto>(findDepartment);

                return Result<DepartmentDto>.Success(departmentDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
