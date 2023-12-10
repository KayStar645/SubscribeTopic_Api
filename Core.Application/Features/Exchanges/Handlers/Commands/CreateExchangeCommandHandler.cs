using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Exchanges;
using Core.Application.DTOs.Exchanges.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Exchanges.Events;
using Core.Application.Features.Exchanges.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Exchanges.Handlers.Commands
{
    public class CreateExchangeCommandHandler : IRequestHandler<CreateExchangeRequest, Result<ExchangeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateExchangeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ExchangeDto>> Handle(CreateExchangeRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateExchangeDtoValidator(_unitOfWork);
            var validationResult = await validator.ValidateAsync(request.createExchangeDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ExchangeDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var exchange = _mapper.Map<Exchange>(request.createExchangeDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateExchangeUpdateExchangeEvent(exchange, httpContextAccessor, unitOfWork));
                    }
                });

                var newExchange = await _unitOfWork.Repository<Exchange>().AddAsync(exchange);
                await _unitOfWork.Save(cancellationToken);

                var ExchangeDto = _mapper.Map<ExchangeDto>(newExchange);

                return Result<ExchangeDto>.Success(ExchangeDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<ExchangeDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ExchangeDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ExchangeDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ExchangeDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
