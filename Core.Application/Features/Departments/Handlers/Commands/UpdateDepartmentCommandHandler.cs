﻿using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Department.Validators;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Departments.Handlers.Commands
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, Result<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateDepartmentDtoValidator();
            var errorValidator = await validator.ValidateAsync(request.UpdateDepartmentDto);

            if(errorValidator.IsValid == false) 
            {
                var errorMessage = errorValidator.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDto>.Failure(errorMessage, (int)HttpStatusCode.BadRequest);
            }


            try
            {
                var findDepartment = await _unitOfWork.Repository<Department>().GetByIdAsync(request.UpdateDepartmentDto.Id);

                if (findDepartment is null)
                {
                    return Result<DepartmentDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.UpdateDepartmentDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findDepartment.CopyPropertiesFrom(request.UpdateDepartmentDto);

                var newDepartment = await _unitOfWork.Repository<Department>().UpdateAsync(findDepartment);
                await _unitOfWork.Save(cancellationToken);

                var departmentDto = _mapper.Map<DepartmentDto>(newDepartment);

                return Result<DepartmentDto>.Success(departmentDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex) 
            {
                return Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }

            throw new NotImplementedException();
        }
    }
}
