using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Application.DTOs.Invitation.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Invitations.Events;
using Core.Application.Features.Invitations.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Invitations.Handlers.Commands
{
    public class SendInvitationCommandHandler : IRequestHandler<SendInvitationRequest, Result<InvitationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public SendInvitationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<InvitationDto>> Handle(SendInvitationRequest request, CancellationToken cancellationToken)
        {
            var validator = new SendInvitationDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.sendInvitationDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<InvitationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var invitation = _mapper.Map<Invitation>(request.sendInvitationDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeSendInvitationUpdateInvitationEvent(invitation, httpContextAccessor, unitOfWork));

                    }
                });

                var newInvitation = await _unitOfWork.Repository<Invitation>().AddAsync(invitation);
                await _unitOfWork.Save(cancellationToken);

                var invitationDto = _mapper.Map<InvitationDto>(newInvitation);

                return Result<InvitationDto>.Success(invitationDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<InvitationDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<InvitationDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<InvitationDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<InvitationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
