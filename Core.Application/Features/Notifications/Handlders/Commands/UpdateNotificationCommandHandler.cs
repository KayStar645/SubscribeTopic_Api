using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Notification;
using Core.Application.DTOs.Notification.Validators;
using Core.Application.Features.Notifications.Requests.Commands;
using Core.Application.Responses;
using Core.Application.Services;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using System.Net;

namespace Core.Application.Features.Notifications.Handlers.Commands
{
    public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationRequest, Result<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateNotificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<NotificationDto>> Handle(UpdateNotificationRequest request, CancellationToken cancellationToken)
        {
            var validator = new UpdateNotificationDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.updateNotificationDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<NotificationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findNotification = await _unitOfWork.Repository<Notification>().GetByIdAsync(request.updateNotificationDto.Id);

                if (findNotification is null)
                {
                    return Result<NotificationDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.updateNotificationDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var mapNoti = _mapper.Map<Notification>(request.updateNotificationDto);

                findNotification.CopyPropertiesFrom(mapNoti);

                var newNotification = await _unitOfWork.Repository<Notification>().UpdateAsync(findNotification);
                await _unitOfWork.Save(cancellationToken);

                var notificationDto = _mapper.Map<NotificationDto>(newNotification);

                return Result<NotificationDto>.Success(notificationDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
