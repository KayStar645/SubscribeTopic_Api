using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.ThesisRegistration;
using Core.Application.DTOs.ThesisRegistration.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.ThesisRegistrations.Event;
using Core.Application.Features.ThesisRegistrations.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.ThesisRegistrations.Handlers.Commands
{
    public class CreateThesisRegistrationCommandHandler : IRequestHandler<CreateThesisRegistrationRequest, Result<ThesisRegistrationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateThesisRegistrationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ThesisRegistrationDto>> Handle(CreateThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreaeThesisRegistrationDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createThesisRegistrationDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisRegistrationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var thesisRegistration = _mapper.Map<ThesisRegistration>(request.createThesisRegistrationDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateThesisRegistrationUpdateThesisRegistrationEvent(thesisRegistration, httpContextAccessor, unitOfWork));

                    }
                });

                var newThesisRegistration = await _unitOfWork.Repository<ThesisRegistration>().AddAsync(thesisRegistration);
                await _unitOfWork.Save(cancellationToken);

                var thesisRegistrationDto = _mapper.Map<ThesisRegistrationDto>(newThesisRegistration);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterCreateThesisRegistrationCreatePointEvent(newThesisRegistration, unitOfWork));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<ThesisRegistrationDto>.Success(thesisRegistrationDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ThesisRegistrationDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ThesisRegistrationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
