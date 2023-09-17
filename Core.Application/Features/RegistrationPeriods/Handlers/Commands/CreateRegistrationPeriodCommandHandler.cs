using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.RegistrationPeriod.Validators;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Commands
{
    public class CreateRegistrationPeriodCommandHandler : IRequestHandler<CreateRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(CreateRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateRegistrationPeriodDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.CreateRegistrationPeriodDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<RegistrationPeriodDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var period = _mapper.Map<RegistrationPeriod>(request.CreateRegistrationPeriodDto);

                var newRegistrationPeriod = await _unitOfWork.Repository<RegistrationPeriod>()
                                                             .AddAsync(period);
                await _unitOfWork.Save(cancellationToken);

                var periodDto = _mapper.Map<RegistrationPeriodDto>(newRegistrationPeriod);

                return Result<RegistrationPeriodDto>.Success(periodDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
