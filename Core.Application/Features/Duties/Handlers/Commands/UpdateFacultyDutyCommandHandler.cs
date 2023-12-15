using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty.Faculty;
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
    public class UpdateFacultyDutyCommandHandler : IRequestHandler<UpdateFacultyDutyRequest, Result<FacultyDutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateFacultyDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<FacultyDutyDto>> Handle(UpdateFacultyDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateDutyDtoValidator(_unitOfWork, request.updateDutyDto.Id);
            var validationResult = await validator.ValidateAsync(request.updateDutyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<FacultyDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findDuty = await _unitOfWork.Repository<Duty>().GetByIdAsync(request.updateDutyDto.Id);

                if (findDuty is null)
                {
                    return Result<FacultyDutyDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.updateDutyDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findDuty.CopyPropertiesFrom(request.updateDutyDto);

                var newDuty = await _unitOfWork.Repository<Duty>().UpdateAsync(findDuty);
                await _unitOfWork.Save(cancellationToken);

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(newDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
