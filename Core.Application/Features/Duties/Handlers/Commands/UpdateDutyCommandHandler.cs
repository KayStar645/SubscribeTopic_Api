using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Duty.Validators;
using Core.Application.Features.Duties.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Commands
{
    public class UpdateDutyCommandHandler : IRequestHandler<UpdateDutyRequest, Result<DutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<DutyDto>> Handle(UpdateDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateDutyDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.updateDutyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findDuty = await _unitOfWork.Repository<Duty>().GetByIdAsync(request.updateDutyDto.Id);

                if (findDuty is null)
                {
                    return Result<DutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateDutyDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findDuty.CopyPropertiesFrom(request.updateDutyDto);

                var newDuty = await _unitOfWork.Repository<Duty>().UpdateAsync(findDuty);
                await _unitOfWork.Save(cancellationToken);

                var dutyDto = _mapper.Map<DutyDto>(newDuty);

                return Result<DutyDto>.Success(dutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<DutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
