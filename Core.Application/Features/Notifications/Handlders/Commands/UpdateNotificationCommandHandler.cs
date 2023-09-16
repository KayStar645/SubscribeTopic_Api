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
            var validationResult = await validator.ValidateAsync(request.UpdateNotificationDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<NotificationDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var findNotification = await _unitOfWork.Repository<Notification>().GetByIdAsync(request.UpdateNotificationDto.Id);

                if (findNotification is null)
                {
                    return Result<NotificationDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.UpdateNotificationDto.Id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                findNotification.CopyPropertiesFrom(request.UpdateNotificationDto);

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
