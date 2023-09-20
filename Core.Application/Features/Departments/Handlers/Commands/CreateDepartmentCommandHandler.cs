using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Department;
using Core.Application.DTOs.Department.Validators;
using Core.Application.Features.Departments.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Departments.Handlers.Commands
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentRequest, Result<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> Handle(CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateDepartmentDtoValidator(_unitOfWork);
            var validatorResult = await validator.ValidateAsync(request.createDepartmentDto);

            if(validatorResult.IsValid == false)
            {
                var errorMessages = validatorResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var department = _mapper.Map<Department>(request.createDepartmentDto);

                var newDepartment = await _unitOfWork.Repository<Department>().AddAsync(department);
                await _unitOfWork.Save(cancellationToken);

                var departmentDto = _mapper.Map<DepartmentDto>(newDepartment);

                return Result<DepartmentDto>.Success(departmentDto, (int)HttpStatusCode.OK);

            }
            catch (Exception ex) 
            {
                return Result<DepartmentDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
        }
    }
}
