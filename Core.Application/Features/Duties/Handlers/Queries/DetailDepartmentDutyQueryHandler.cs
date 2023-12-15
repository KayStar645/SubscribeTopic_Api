using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty.Faculty;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Queries
{
    public class DetailDepartmentDutyQueryHandler : IRequestHandler<DetailDepartmentDutyRequest, Result<DepartmentDutyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailDepartmentDutyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
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
                var query = _unitOfWork.Repository<Duty>().GetByIdInclude(request.id)
                                        .Where(x => x.Type == Duty.TYPE_DEPARTMENT);

                if (request.isAllDetail || request.isGetDepartment == true)
                {
                    query = query.Include(x => x.Department);
                }
                if (request.isAllDetail || request.isGetTeacher == true)
                {
                    query = query.Include(x => x.Teacher);
                }
                if (request.isAllDetail || request.isGetForDuty == true)
                {
                    query = query.Include(x => x.ForDuty);
                }

                var findDuty = await query.SingleOrDefaultAsync();

                if (findDuty is null)
                {
                    return Result<DepartmentDutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var DepartmentDutyDto = _mapper.Map<DepartmentDutyDto>(findDuty);

                return Result<DepartmentDutyDto>.Success(DepartmentDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
