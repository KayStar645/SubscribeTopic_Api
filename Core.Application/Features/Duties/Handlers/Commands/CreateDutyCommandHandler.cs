using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Duty;
using Core.Application.DTOs.Duty.Validators;
using Core.Application.Features.Duties.Events;
using Core.Application.Features.Duties.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Duties.Handlers.Commands
{
    public class CreateDutyCommandHandler : IRequestHandler<CreateDutyRequest, Result<DutyDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateDutyCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<DutyDto>> Handle(CreateDutyRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateDutyDtoValidator(_unitOfWork, request?.createDutyDto?.Type);
            var validationResult = await validator.ValidateAsync(request.createDutyDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<DutyDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var duty = _mapper.Map<Duty>(request.createDutyDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateDutyUpdateDutyEvent(duty, httpContextAccessor, unitOfWork));

                    }
                });

                var newDuty = await _unitOfWork.Repository<Duty>().AddAsync(duty);
                await _unitOfWork.Save(cancellationToken);

                var dutyDto = _mapper.Map<DutyDto>(newDuty);

                return Result<DutyDto>.Success(dutyDto, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return Result<DutyDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
