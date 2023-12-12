using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.DTOs.Duty;
using Core.Application.Features.Duties.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Queries
{
    public class DetailDutyQueryHandler : IRequestHandler<DetailDutyRequest, Result<DutyDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailDutyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<DutyDto>> Handle(DetailDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new DetailBaseRequestValidator();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DutyDto>.Failure(string.Join(", ", errorMessages), (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var query = _unitOfWork.Repository<Duty>().GetByIdInclude(request.id);

                if (request.isAllDetail || request.isGetFaculty == true)
                {
                    query = query.Include(x => x.Faculty);
                }
                if (request.isAllDetail || request.isGetDepartment == true)
                {
                    query = query.Include(x => x.Department);
                }
                if (request.isAllDetail || request.isGetTeacher == true)
                {
                    query = query.Include(x => x.Teacher);
                }

                var findDuty = await query.SingleAsync();

                if (findDuty is null)
                {
                    return Result<DutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var dutyDto = _mapper.Map<DutyDto>(findDuty);

                return Result<DutyDto>.Success(dutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
