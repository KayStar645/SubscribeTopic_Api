using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Notification;
using Core.Application.DTOs.Notification.Validators;
using Core.Application.Features.Notifications.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Notifications.Handlers.Commands
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationRequest, Result<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateNotificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<NotificationDto>> Handle(CreateNotificationRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateNotificationDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createNotificationDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<NotificationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var notification = _mapper.Map<Notification>(request.createNotificationDto);

                var newNotification = await _unitOfWork.Repository<Notification>().AddAsync(notification);
                await _unitOfWork.Save(cancellationToken);

                var notificationDto = _mapper.Map<NotificationDto>(newNotification);

                return Result<NotificationDto>.Success(notificationDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
