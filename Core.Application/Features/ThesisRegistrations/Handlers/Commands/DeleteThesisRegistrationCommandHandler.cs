using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Common.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.ThesisRegistrations.Event;
using Core.Application.Features.ThesisRegistrations.Requests.Commands;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Features.ThesisRegistrations.Handlers.Commands
{
    public class DeleteThesisRegistrationCommandHandler : IRequestHandler<DeleteThesisRegistrationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;


        public DeleteThesisRegistrationCommandHandler(IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
        }

        public virtual async Task<Unit> Handle(DeleteThesisRegistrationRequest request, CancellationToken cancellationToken)
        {
            var validator = new DeleteBaseRequestValidator<ThesisRegistration>();
            var result = await validator.ValidateAsync(request);

            if (result.IsValid == false)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                throw new BadRequestException(string.Join(",", errorMessages));
            }

            var entity = await _unitOfWork.Repository<ThesisRegistration>().GetByIdAsync(request.id);

            await Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    await mediator.Publish(new BeforeDeleteThesisRegistrationCheckRoleEvent(entity, httpContextAccessor, unitOfWork));

                }
            });

            if (entity == null)
                throw new NotFoundException("id", request.id.ToString());

            await _unitOfWork.Repository<ThesisRegistration>().DeleteAsync(entity);
            await _unitOfWork.Save();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    await mediator.Publish(new AfterDeleteThesisRegistrationDeletePointEvent(entity, unitOfWork));

                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            return Unit.Value;
        }
    }
}
