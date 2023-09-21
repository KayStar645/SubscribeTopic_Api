using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Notification;
using Core.Application.Features.Notifications.Requests.Queries;
using Core.Application.Responses;
using Core.Application.Transform;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Core.Application.Features.Notifications.Handlers.Queries
{
    public class DetailNotificationRequestHandler : IRequestHandler<DetailNotificationRequest, Result<NotificationDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public DetailNotificationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<NotificationDto>> Handle(DetailNotificationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<Notification>().GetByIdInclude(request.id);

                if (request.isAllDetail)
                {
                    query = _unitOfWork.Repository<Notification>().AddInclude(query, x => x.Faculty);
                }
                else
                {
                    if (request.isGetFaculty == true)
                    {
                        query = _unitOfWork.Repository<Notification>().AddInclude(query, x => x.Faculty);
                    }
                }

                var findNotification = await query.SingleAsync();

                if (findNotification is null)
                {
                    return Result<NotificationDto>.Failure(
                        ValidatorTranform.NotExistsValue("Id", request.id.ToString()),
                        (int)HttpStatusCode.NotFound
                    );
                }

                var NotificationDto = _mapper.Map<NotificationDto>(findNotification);

                return Result<NotificationDto>.Success(NotificationDto, (int)HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Result<NotificationDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
