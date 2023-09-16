using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.Exceptions;
using Core.Application.Features.Notifications.Requests.Commands;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Notifications.Handlers.Commands
{
    public class DeleteNotificationCommandHanler : IRequestHandler<DeleteNotificationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteNotificationCommandHanler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(DeleteNotificationRequest request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(request.Id);

            if (notification is null)
            {
                throw new NotFoundException(nameof(Notification), request.Id);
            }

            await _unitOfWork.Repository<Notification>().DeleteAsync(notification);
            await _unitOfWork.Save(cancellationToken);

            return Unit.Value;
        }
    }
}
