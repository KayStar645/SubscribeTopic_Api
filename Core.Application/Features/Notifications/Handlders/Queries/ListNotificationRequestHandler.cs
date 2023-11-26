using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Notification;
using Core.Application.Features.Notifications.Requests.Queries;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services.Interface;
using System.Net;

namespace Core.Application.Features.Notifications.Handlders.Queries
{
    public class ListNotificationRequestHandler : IRequestHandler<ListNotificationRequest, PaginatedResult<List<NotificationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;

        public ListNotificationRequestHandler(IUnitOfWork unitOfWork, IMapper mapper, ISieveProcessor sieveProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
        }

        public async Task<PaginatedResult<List<NotificationDto>>> Handle(ListNotificationRequest request, CancellationToken cancellationToken)
        {
            var validator = new ListNotificationDtoValidator(_unitOfWork);
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                return PaginatedResult<List<NotificationDto>>
                    .Failure((int)HttpStatusCode.BadRequest, errorMessages);
            }

            var sieve = _mapper.Map<SieveModel>(request);

            var query = _unitOfWork.Repository<Notification>().GetAllInclude();

            if(request.facultyId != null)
            {
                query = query.Where(x => x.FacultyId == request.facultyId);
            }   
            else
            {
                query = query.Where(x => x.FacultyId == null);
            }

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

            query = _sieveProcessor.Apply(sieve, query);

            int totalCount = await query.CountAsync();

            var notifications = await query.ToListAsync();

            var mapNotifications = _mapper.Map<List<NotificationDto>>(notifications);

            return PaginatedResult<List<NotificationDto>>.Success(
                mapNotifications, totalCount, request.page,
                request.pageSize);
        }
    }
}
