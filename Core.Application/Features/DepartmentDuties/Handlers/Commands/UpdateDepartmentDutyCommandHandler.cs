using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.DTOs.DepartmentDuty.Validators;
using Core.Application.Features.DepartmentDuties.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.DepartmentDuties.Handlers.Commands
{
    public class UpdateDepartmentDutyCommandHandler : IRequestHandler<UpdateDepartmentDutyRequest, Result<DepartmentDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDepartmentDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDutyDto>> Handle(UpdateDepartmentDutyRequest request, CancellationToken cancellationToken)
        {
            if(request.UpdateDepartmentDutyDto.Id == null)
            {
                return Result<DepartmentDutyDto>.Failure(
                    ValidatorTranform.Required("id"),
                    (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findDepartmentDuty = await _unitOfWork.Repository<DepartmentDuty>().GetByIdAsync(request.UpdateDepartmentDutyDto.Id);

                if (findDepartmentDuty is null)
                {
                    return Result<DepartmentDutyDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.UpdateDepartmentDutyDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var validator = new UpdateDepartmentDutyDtoValidator(_unitOfWork,
                request.UpdateDepartmentDutyDto.Id,
                findDepartmentDuty.DepartmentId,
                request.UpdateDepartmentDutyDto.TimeStart ?? DateTime.Now);
                var validationResult = await validator.ValidateAsync(request.UpdateDepartmentDutyDto);

                if (validationResult.IsValid == false)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                    return Result<DepartmentDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
                }

                findDepartmentDuty.CopyPropertiesFrom(request.UpdateDepartmentDutyDto);

                var newDepartmentDuty = await _unitOfWork.Repository<DepartmentDuty>().UpdateAsync(findDepartmentDuty);
                await _unitOfWork.Save(cancellationToken);

                var DepartmentDutyDto = _mapper.Map<DepartmentDutyDto>(newDepartmentDuty);

                return Result<DepartmentDutyDto>.Success(DepartmentDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
