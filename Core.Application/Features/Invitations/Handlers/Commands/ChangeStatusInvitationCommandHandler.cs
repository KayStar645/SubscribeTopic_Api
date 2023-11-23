using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Invitation;
using Core.Application.DTOs.Invitation.Validators;
using Core.Application.Features.Invitations.Events;
using Core.Application.Features.Invitations.Request.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Invitations.Handlers.Commands
{
    public class ChangeStatusInvitationCommandHandler : IRequestHandler<ChangeStatusInvitationRequest, Result<InvitationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IServiceProvider _serviceProvider;

        public ChangeStatusInvitationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContext, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContext = httpContext;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<InvitationDto>> Handle(ChangeStatusInvitationRequest request, CancellationToken cancellationToken)
        {
            var validator = new ChangeStatusInvitationDtoValidator(_unitOfWork,
                                (int)request.changeStatusInvitation.Id, _httpContext);
            var validationResult = await validator.ValidateAsync(request.changeStatusInvitation);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<InvitationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findInvitation = await _unitOfWork.Repository<Invitation>().GetByIdAsync(request.changeStatusInvitation.Id);

                if (findInvitation is null)
                {
                    return Result<InvitationDto>.Failure(
                        ValidatorTransform.NotExistsValue("Id", request.changeStatusInvitation.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findInvitation.CopyPropertiesFrom(request.changeStatusInvitation);

                var newInvitation = await _unitOfWork.Repository<Invitation>().UpdateAsync(findInvitation);
                await _unitOfWork.Save(cancellationToken);

                var invitationDto = _mapper.Map<InvitationDto>(newInvitation);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                        await mediator.Publish(new AfterChangeStatusInvitationChangeInvitationEnvent(invitationDto, unitOfWork, mapper));
                        await mediator.Publish(new AfterChangeStatusInvitationChangeGroupEnvent(invitationDto, unitOfWork, mapper));
                        await mediator.Publish(new AfterChangeStatusInvitationChangeStudentJoinEnvent(invitationDto, unitOfWork, mapper));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<InvitationDto>.Success(invitationDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<InvitationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
