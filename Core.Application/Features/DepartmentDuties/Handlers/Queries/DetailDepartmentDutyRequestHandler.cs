using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.DTOs.Department;
using Core.Application.Features.DepartmentDuties.Requests.Queries;
using Core.Application.Interfaces.Repositories;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.DepartmentDuties.Handlers.Queries
{
    public class DetailDepartmentDutyRequestHandler : IRequestHandler<DetailDepartmentDutyRequest, Result<DepartmentDutyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailDepartmentDutyRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DepartmentDutyDto>> Handle(DetailDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DepartmentDutyDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<DepartmentDuty>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
                }
                else
                {
                    if (request.isGetDepartment == true)
                    {
                        query = _unitOfWork.Repository<DepartmentDuty>().AddInclude(query, x => x.Department);
                    }
                }

                var findDepartmentDuty = await query.SingleOrDefaultAsync();

                if (findDepartmentDuty is null)
                {
                    return Result<DepartmentDutyDto>.Failure(
                        ValidatorTranform.NotExistsValue("id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var DepartmentDutyDto = _mapper.Map<DepartmentDutyDto>(findDepartmentDuty);

                return Result<DepartmentDutyDto>.Success(DepartmentDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
