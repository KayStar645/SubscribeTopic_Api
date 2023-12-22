using AutoMapper;
using Core.Application.Contracts.Persistence;
using Core.Application.DTOs.Thesis;
using Core.Application.DTOs.Thesis.Validators;
using Core.Application.Exceptions;
using Core.Application.Features.Thesiss.Events;
using Core.Application.Features.Thesiss.Requests.Commands;
using Core.Application.Responses;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.Application.Features.Thesiss.Handlers.Commands
{
    public class CreateThesisCommandHandler : IRequestHandler<CreateThesisRequest, Result<ThesisDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;

        public CreateThesisCommandHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<Result<ThesisDto>> Handle(CreateThesisRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateThesisDtoValidator(_unitOfWork, request.createThesisDto.MinQuantity);
            var validationResult = await validator.ValidateAsync(request.createThesisDto);

            if (validationResult.IsValid == false)
            {
                var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                return Result<ThesisDto>.Failure(errorMessages, (int)HttpStatusCode.BadRequest);
            }

            try
            {
                var thesis = _mapper.Map<Thesis>(request.createThesisDto);

                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new BeforeCreateThesisUpdateThesisEvent(thesis, httpContextAccessor, unitOfWork));

                    }
                });

                var newThesis = await _unitOfWork.Repository<Thesis>().AddAsync(thesis);
                await _unitOfWork.Save(cancellationToken);

                var thesisDto = _mapper.Map<ThesisDto>(newThesis);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await Task.Run(async () =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        await mediator.Publish(new AfterCreateThesisCreateThesisMajorsEvent(request.createThesisDto, thesisDto, unitOfWork));

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return Result<ThesisDto>.Success(thesisDto, (int)HttpStatusCode.Created);
            }
            catch (NotFoundException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.NotFound);
            }
            catch (BadRequestException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedException ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Result<ThesisDto>.Failure(ex.Message, (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
