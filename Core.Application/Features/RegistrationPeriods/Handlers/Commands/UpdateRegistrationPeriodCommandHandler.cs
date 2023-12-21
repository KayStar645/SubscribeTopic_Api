using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.RegistrationPeriod;
using Core.Application.DTOs.RegistrationPeriod.Validators;
using Core.Application.DTOs.Thesis;
using Core.Application.Features.RegistrationPeriods.Event;
using Core.Application.Features.RegistrationPeriods.Requests.Commands;
using Core.Application.Features.Thesiss.Events;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.RegistrationPeriods.Handlers.Commands
{
    public class UpdateRegistrationPeriodCommandHandler : IRequestHandler<UpdateRegistrationPeriodRequest, Result<RegistrationPeriodDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public UpdateRegistrationPeriodCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<RegistrationPeriodDto>> Handle(UpdateRegistrationPeriodRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateRegistrationPeriodDtoValidator(_unitOfWork);
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterUpdateRegistrationPeriodUpdateRegistrationPeriodEvent(periodDto, unitOfWork));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<RegistrationPeriodDto>.Success(periodDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<RegistrationPeriodDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
