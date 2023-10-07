using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Base.Requests.Commands;
using Core.Domain.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Application.Features.Base.Handlers.Commands
{
    public class DeleteBaseCommandHandler<T> : IRequestHandler<DeleteBaseRequest<T>, Unit>
        where T : BaseAuditableEntity
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public DeleteBaseCommandHandler(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        public virtual async Task<Unit> Handle(DeleteBaseRequest<T> request, CancellationToken cancellationToken)
        {
            var validator = new DeleteBaseRequestValidator<T>();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(",", errorMessages));
            }

            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(request.id);

            if (entity == null)
                throw new NotFoundException("id", request.id.ToString());

            await _unitOfWork.Repository<T>().DeleteAsync(entity);
            await _unitOfWork.Save(cancellationToken);

            Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    string eventName = $"AfterDeleted{entity.GetType().Name}Event";

                    List<Type> customEventHandlers = Assembly.GetExecutingAssembly().GetTypes()
                        .Where(type => type.Name.EndsWith(eventName))
                        .ToList();

                    foreach (var customEventHandlerType in customEventHandlers)
                    {
                        if (typeof(INotification).IsAssignableFrom(customEventHandlerType))
                        {
                            // Lấy tên của lớp xử lý sự kiện
                            string eventHandlerName = customEventHandlerType.Name;

                            // Tạo một đối tượng từ tên lớp và truyền tham số vào constructor
                            var eventHandlerInstance = (INotification)Activator.CreateInstance(customEventHandlerType, entity, unitOfWork);

                            // Gửi sự kiện
                            await mediator.Publish(eventHandlerInstance);
                        }
                    }
                }
            });

            return Unit.Value;
        }
    }
}
