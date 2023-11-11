using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.RegistrationPeriod.Validators;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Commands
{
    public class UpdateRegistrationPeriodCommandHandler : IRequestHandler<UpdateRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(UpdateRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateRegistrationPeriodDtoValidator(_unitOfWork,
                request.UpdateRegistrationPeriodDto.TimeStart ?? DateTime.Now);
            var validationResult = await validator.ValidateAsync(request.UpdateRegistrationPeriodDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<RegistrationPeriodDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findPeriod = await _unitOfWork.Repository<RegistrationPeriod>().GetByIdAsync(request.UpdateRegistrationPeriodDto.Id);

                if (findPeriod is null)
                {
                    return Result<RegistrationPeriodDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.UpdateRegistrationPeriodDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findPeriod.CopyPropertiesFrom(request.UpdateRegistrationPeriodDto);

                var newPeriod = await _unitOfWork.Repository<RegistrationPeriod>().UpdateAsync(findPeriod);
                await _unitOfWork.Save(cancellationToken);

                var periodDto = _mapper.Map<RegistrationPeriodDto>(newPeriod);

                return Result<RegistrationPeriodDto>.Success(periodDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
