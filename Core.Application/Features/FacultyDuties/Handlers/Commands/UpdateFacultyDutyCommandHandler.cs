using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.DepartmentDuty;
using Core.Application.DTOs.FacultyDuty;
using Core.Application.DTOs.FacultyDuty.Validators;
using Core.Application.Features.FacultyDuties.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.FacultyDuties.Handlers.Commands
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
            if (request.UpdateFacultyDutyDto.Id == null)
            {
                return Result<FacultyDutyDto>.Failure(
                    ValidatorTranform.Required("id"),
                    (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findFacultyDuty = await _unitOfWork.Repository<FacultyDuty>().GetByIdAsync(request.UpdateFacultyDutyDto.Id);

                if (findFacultyDuty is null)
                {
                    return Result<FacultyDutyDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.UpdateFacultyDutyDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var validator = new UpdateFacultyDutyDtoValidator(_unitOfWork,
                request.UpdateFacultyDutyDto.Id,
                findFacultyDuty.FacultyId,
                request.UpdateFacultyDutyDto.TimeStart ?? DateTime.Now);
                var validationResult = await validator.ValidateAsync(request.UpdateFacultyDutyDto);

                if (validationResult.IsValid == false)
                {
                    var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                    return Result<FacultyDutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
                }

                findFacultyDuty.CopyPropertiesFrom(request.UpdateFacultyDutyDto);

                var newFacultyDuty = await _unitOfWork.Repository<FacultyDuty>().UpdateAsync(findFacultyDuty);
                await _unitOfWork.Save(cancellationToken);

                var FacultyDutyDto = _mapper.Map<FacultyDutyDto>(newFacultyDuty);

                return Result<FacultyDutyDto>.Success(FacultyDutyDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<FacultyDutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
