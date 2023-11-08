using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Department;
using Core.Application.Features.Departments.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using FluentValidation;
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
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Department>().GetByIdInclude(request.id);

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

                var findDepartment = await query.SingleOrDefaultAsync();

                if (findDepartment is null)
                {
                    return Result<DepartmentDto>.Failure(
                        ValidatorTransform.NotExistsValue("id", request.id.ToString()),
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
